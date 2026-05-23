using AdList.Application.Abstract;
using AdList.Application.Abstract.Command;
using AdList.Application.Behaviors;
using AdList.Application.Commands.CreateSmartTaskCommand;
using AdList.Application.Commands.CreateUserCommand;
using AdList.Application.Commands.UpdateSmartTaskCommand;
using AdList.Application.Infrastructure;
using AdList.Application.Infrastructure.Processors;
using AdList.DataAccess;
using AdList.DataAccess.Repositories;
using AdList.Domain;
using AdList.Infrastructure;
using AdList.Web.Endpoints;
using AdList.Web.Infrastructure;
using AdList.Web.Infrastructure.Swagger;
using AdList.Web.Middlewares;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace AdList.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json", false, true);
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", false, true);

#if DEBUG
        builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);
#endif

        builder.Configuration.AddEnvironmentVariables();

        // Configure services
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.Configure<RouteHandlerOptions>(options =>
        {
            // By default no BadHttpRequestException exception is throw in non-development environment.
            // Configure options to throw the exception in all environments and handle it into the exception handler middleware and write a correct response.
            options.ThrowOnBadRequest = true;
        });

        builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));

        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddHealthChecks();

        builder.Services.AddDbContext<EFContext>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IFluentValidator, CreateSmartTaskCommandValidator>();
        builder.Services.AddScoped<IFluentValidator, UpdateSmartTaskCommandValidator>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<ISmartTaskRepository, SmartTaskRepository>();
        builder.Services.AddScoped<IPrincipalProvider, PrincipalProvider>();
        builder.Services.AddMemoryCache();

        builder.Services.AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
            cfg.AddOpenBehavior(typeof(RequestPreProcessorBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(CommitBehavior<,>));
        });

        builder.Services.AddScoped(typeof(IRequestPreProcessor<>), typeof(PrincipalPreProcessor<>));

        var audience = builder.Configuration["okta:audience"];
        var authority = builder.Configuration["okta:authority"];

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();

                        string? userEmail = context.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                        string? userName = context.Principal?.Claims.FirstOrDefault(c => c.Type == Constants.Claims.Name)?.Value;

                        return mediator.Send(new CreateUserCommand(userName, userEmail));
                    }
                };
            });

        builder.Services.AddAuthorization();

        // Add minimal services (placeholder for later DI)
        builder.Services.AddEndpointsApiExplorer();
        // Enable Swagger/OpenAPI generation
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Type = SecuritySchemeType.Http,
            });

            options.DocumentFilter<ExceptionReasonCodeDocumentFilter>();
            options.DocumentFilter<ClaimsPrincipalDocumentFilter>();
            options.SchemaFilter<EnumSchemaFilter>();
            options.SchemaFilter<CurrentPrincipalSchemaFilter>();

            options.OperationFilter<AuthorizeOperationFilter>();

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "AdList.Web"
            });
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        // Enable Swagger UI in development
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("v1/swagger.json", "Server API Schema v1"));
        }

        app.UseMiddleware<ExceptionHandlerMiddleware>();

        app.UseHealthChecks("/health/ping", new HealthCheckOptions
        {
            ResponseWriter = (ctx, _) => ctx.Response.WriteAsync("pong")
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapEndpoints();

        await app.RunAsync();
    }
}

using AdList.Application.Abstract.Command;
using AdList.Application.Abstract.Implementation;
using AdList.Application.Commands.CompleteSmartTaskCommand;
using AdList.Application.Commands.CreateSmartTaskCommand;
using AdList.Application.Commands.DeleteSmartTaskCommand;
using AdList.Application.Commands.GetTasksCommand;
using AdList.Application.Commands.UpdateSmartTaskCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdList.Web.Endpoints
{
    public static partial class Endpoints
    {
        private static void MapCommands(IEndpointRouteBuilder app)
        {
            app.MapPost("/create-task", InvokeCommandAsync<CreateSmartTaskCommand, EmptyResponse>())
                .WithTags(OpenApiTag)
                .RequireAuthorization();

            app.MapPut("/update-task", InvokeCommandAsync<UpdateSmartTaskCommand, EmptyResponse>())
                .WithTags(OpenApiTag)
                .RequireAuthorization();

            app.MapDelete("/delete-task", InvokeCommandAsync<DeleteSmartTaskCommand, EmptyResponse>())
                .WithTags(OpenApiTag)
                .RequireAuthorization();

            app.MapPut("/complete-task", InvokeCommandAsync<CompleteSmartTaskCommand, EmptyResponse>())
                .WithTags(OpenApiTag)
                .RequireAuthorization();

            app.MapPost("/get-tasks", InvokeCommandAsync<GetTasksCommand, GetTasksCommandResponse>())
                .WithTags(OpenApiTag)
                .RequireAuthorization();

            return;

            Delegate InvokeCommandAsync<TCommand, TCommandResponse>()
                where TCommand : ICommand<TCommandResponse>
                where TCommandResponse : ICommandResponse
            {
                return ([FromBody] TCommand command,
                    [FromServices] IMediator mediator,
                    CancellationToken cancellationToken) => mediator.Send(command, cancellationToken);
            }
        }
    }
}

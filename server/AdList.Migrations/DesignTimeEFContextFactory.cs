using AdList.DataAccess;
using AdList.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AdList.Migrations;

public class DesignTimeEFContextFactory : IDesignTimeDbContextFactory<EFContext>
{  
    public static EFContext CreateDbContext()
    {
        DatabaseOptions options = StaticSettings.DatabaseOptions;
        string connectionString = options.ConnectionStrings["Default"];
        
        var optionsBuilder = new DbContextOptionsBuilder<EFContext>();
        optionsBuilder.UseNpgsql(connectionString, builder =>
        {
            builder.MigrationsAssembly(typeof(DesignTimeEFContextFactory).Assembly.FullName);

            if (StaticSettings.DatabaseOptions.SqlCommandTimeout is > 0)
            {
                builder.CommandTimeout(StaticSettings.DatabaseOptions.SqlCommandTimeout);
            }
        });

        return new EFContext(optionsBuilder.Options, null, null!, new DateTimeProvider());
    }

    public EFContext CreateDbContext(string[] args)
    {
        return DesignTimeEFContextFactory.CreateDbContext();
    }
}
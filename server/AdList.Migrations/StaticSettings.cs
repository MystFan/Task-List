using AdList.DataAccess;
using Microsoft.Extensions.Configuration;

namespace AdList.Migrations;

internal static class StaticSettings
{
    public static DatabaseOptions DatabaseOptions { get; } = new();

    static StaticSettings()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
#if DEBUG
            .AddJsonFile("appsettings.Development.json", true)
#else
            .AddJsonFile("appsettings.Production.json", optional: true)
#endif
            .AddEnvironmentVariables()
            .Build();

        configuration.GetSection("database").Bind(DatabaseOptions);
    }
}
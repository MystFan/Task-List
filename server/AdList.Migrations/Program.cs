using AdList.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AdList.Migrations
{
    internal static class Program
    {
        private static async Task<int> Main()
        {
            try
            {
                await using EFContext dbContext = DesignTimeEFContextFactory.CreateDbContext();
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception)
            {
                return -1;
            }

            return 0;
        }
    }
}

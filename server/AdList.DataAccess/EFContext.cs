using AdList.Application.Infrastructure;
using AdList.Domain.Abstract;
using AdList.Domain.Entities;
using AdList.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AdList.DataAccess
{
    public class EFContext(DbContextOptions<EFContext> options, IOptions<DatabaseOptions>? databaseOptions, IPrincipalProvider principalProvider, IDateTimeProvider dateTimeProvider) : DbContext(options)
    {
        public DbSet<SmartTask> SmartTasks { get; set; } = null!;

        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (databaseOptions is null)
            {
                return;
            }

            DatabaseOptions options = databaseOptions.Value;
            string connectionString = options.ConnectionStrings["Default"];

            optionsBuilder.UseNpgsql(connectionString, builder =>
            {
                if (databaseOptions.Value.SqlCommandTimeout is > 0)
                {
                    builder.CommandTimeout(options.SqlCommandTimeout);
                }
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EFContext).Assembly);
        }

        public override int SaveChanges()
        {
            SetTrackingProperties();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetTrackingProperties();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetTrackingProperties();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SetTrackingProperties();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void SetTrackingProperties()
        {
            var utcNow = dateTimeProvider.UtcNow;

            foreach (var entry in ChangeTracker.Entries<TrackingEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.Author = GetCurrentUser();
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAt = utcNow;
                }
            }
        }

        private string GetCurrentUser()
        {
            Claim? email = principalProvider.Current?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            return email?.Value ?? "System";
        }
    }
}

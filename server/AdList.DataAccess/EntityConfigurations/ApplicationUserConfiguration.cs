using AdList.Domain;
using AdList.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdList.DataAccess.EntityConfigurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable(nameof(ApplicationUser));

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(Constants.ApplicationUser.EmailMaxLength);

            builder.HasIndex(e => e.Email).IsUnique();

            builder.Property(e => e.Name)
                .HasMaxLength(Constants.ApplicationUser.NameMaxLength);
        }
    }
}

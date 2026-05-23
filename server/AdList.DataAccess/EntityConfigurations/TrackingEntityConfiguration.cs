using AdList.Domain;
using AdList.Domain.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdList.DataAccess.EntityConfigurations
{
    public class TrackingEntityConfiguration<T> : IEntityTypeConfiguration<T>
        where T : TrackingEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(e => e.Author)
                .IsRequired()
                .HasMaxLength(Constants.TrackingEntity.AuthorMaxLength);
        }
    }
}

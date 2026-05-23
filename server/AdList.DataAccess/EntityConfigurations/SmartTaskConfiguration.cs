using AdList.Domain;
using AdList.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdList.DataAccess.EntityConfigurations
{
    public class SmartTaskConfiguration : TrackingEntityConfiguration<SmartTask>
    {
        public override void Configure(EntityTypeBuilder<SmartTask> builder)
        {
            builder.ToTable(nameof(SmartTask));

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(Constants.SmartTask.TitleMaxLength);

            builder.Property(e => e.Description)
                .HasMaxLength(Constants.SmartTask.DescriptionMaxLength);

            builder.Property(e => e.DueDate)
                .HasColumnType("timestamp without time zone");

            builder.Property(e => e.CompletionStatus)
                .HasDefaultValue(CompletionStatus.Pending);

            base.Configure(builder);
        }
    }
}

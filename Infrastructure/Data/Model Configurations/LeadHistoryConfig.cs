using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class LeadHistoryConfig : IEntityTypeConfiguration<SalesLeadHistory>
    {
        public void Configure(EntityTypeBuilder<SalesLeadHistory> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(h => h.PropertyName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.OldValue)
                .IsRequired(false)
                .HasMaxLength(2000);

            builder.Property(h => h.NewValue)
                .IsRequired(false)
                .HasMaxLength(2000);

            builder.Property(h => h.UpdatedById)
                .IsRequired(false);

            builder.Property(h => h.UpdatedByName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(h => h.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(h => h.SalesLead)
                .WithMany(l => l.LeadHistory)
                .HasForeignKey(h => h.SalesLeadId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.UpdatedBy)
                .WithMany()
                .HasForeignKey(h => h.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

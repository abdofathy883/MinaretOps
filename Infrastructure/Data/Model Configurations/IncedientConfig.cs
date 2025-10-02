using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class IncedientConfig : IEntityTypeConfiguration<KPIIncedint>
    {
        public void Configure(EntityTypeBuilder<KPIIncedint> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(i => i.EmployeeId)
                .IsRequired();

            builder.Property(i => i.Aspect)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(i => i.TimeStamp)
                .IsRequired();

            builder.Property(i => i.Description)
                .HasMaxLength(2000)
                .IsRequired(false);

            builder.Property(i => i.EvidenceURL)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(i => i.Date)
                .IsRequired(false)
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(i => i.Employee)
                .WithMany(u => u.KPIIncedints)
                .HasForeignKey(i => i.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

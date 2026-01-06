using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class SalaryPaymentConfig : IEntityTypeConfiguration<SalaryPayment>
    {
        public void Configure(EntityTypeBuilder<SalaryPayment> builder)
        {
            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(sp => sp.EmployeeId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(sp => sp.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(sp => sp.Notes)
                .HasMaxLength(3000);

            builder.Property(sp => sp.SalaryPeriodId)
                .IsRequired(false);

            builder.Property(sp => sp.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship with ApplicationUser (Employee)
            builder.HasOne(sp => sp.Employee)
                .WithMany(u => u.SalaryPayments)
                .HasForeignKey(sp => sp.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with SalaryPeriod (optional)
            builder.HasOne(sp => sp.SalaryPeriod)
                .WithMany(sp => sp.SalaryPayments)
                .HasForeignKey(sp => sp.SalaryPeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(sp => sp.EmployeeId);
            builder.HasIndex(sp => sp.SalaryPeriodId);
        }
    }
}

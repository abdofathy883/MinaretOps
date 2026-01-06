using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class SalaryPeriodConfig : IEntityTypeConfiguration<SalaryPeriod>
    {
        public void Configure(EntityTypeBuilder<SalaryPeriod> builder)
        {
            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(sp => sp.EmployeeId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(sp => sp.MonthLabel)
                .IsRequired()
                .HasMaxLength(20); // Format: "YYYY-MM"

            builder.Property(sp => sp.Month)
                .IsRequired();

            builder.Property(sp => sp.Year)
                .IsRequired();

            builder.Property(sp => sp.Salary)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(sp => sp.Bonus)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(sp => sp.Deductions)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(sp => sp.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(sp => sp.UpdatedAt)
                .IsRequired(false);

            builder.Property(sp => sp.Notes)
                .IsRequired(false)
                .HasMaxLength(3000);

            // Relationship with ApplicationUser (Employee)
            builder.HasOne(sp => sp.Employee)
                .WithMany(u => u.SalaryPeriods)
                .HasForeignKey(sp => sp.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with SalaryPayments
            builder.HasMany(sp => sp.SalaryPayments)
                .WithOne(sp => sp.SalaryPeriod)
                .HasForeignKey(sp => sp.SalaryPeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(sp => sp.EmployeeId);
            builder.HasIndex(sp => sp.MonthLabel);
            builder.HasIndex(sp => new { sp.EmployeeId, sp.Year, sp.Month })
                .IsUnique(); // Ensure one period per employee per month/year
        }
    }
}

using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Model_Configurations
{
    public class LeaveRequestConfig : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.HasKey(lr => lr.Id);

            builder.Property(lr => lr.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(lr => lr.EmployeeId)
                .IsRequired();

            builder.Property(lr => lr.FromDate)
                .IsRequired();

            builder.Property(lr => lr.ToDate)
                .IsRequired();

            builder.Property(lr => lr.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(lr => lr.ActionDate)
                .IsRequired(false);

            builder.HasOne(lr => lr.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

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
    public class ComplaintConfig : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(c => c.EmployeeId)
                .IsRequired();

            builder.Property(c => c.Subject)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(2000);

            // Relationship with ApplicationUser (Employee)
            builder.HasOne(c => c.User)
                .WithMany(u => u.Complaints)
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add index for foreign key
            builder.HasIndex(c => c.EmployeeId);
        }
    }
}

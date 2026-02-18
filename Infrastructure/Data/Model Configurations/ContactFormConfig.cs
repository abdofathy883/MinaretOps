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
    public class ContactFormConfig : IEntityTypeConfiguration<ContactEntry>
    {
        public void Configure(EntityTypeBuilder<ContactEntry> builder)
        {
            builder.Property(c => c.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Email)
                .HasMaxLength(200);

            builder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Message)
                .HasMaxLength(3000);

            builder.Property(c => c.IpAddress)
                .HasMaxLength(45);

            builder.Property(c => c.UserAgent)
                .HasMaxLength(500);
        }
    }
}

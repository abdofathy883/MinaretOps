using Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class ContactFormConfig : IEntityTypeConfiguration<ContactFormEntry>
    {
        public void Configure(EntityTypeBuilder<ContactFormEntry> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(c => c.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.CompanyName)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(c => c.Email)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(c => c.PhoneNumber)
                .IsRequired(false)
                .HasMaxLength(20);

            builder.Property(c => c.DesiredService)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(c => c.ProjectBrief)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.Property(c => c.TimeStamp)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
        }
    }
}

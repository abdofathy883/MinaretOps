using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class ServiceConfig : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .UseIdentityColumn(1, 1);

            // Configure properties
            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .IsRequired(false)
                .HasMaxLength(2000);

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.UpdatedAt);

            builder.Property(s => s.IsDeleted)
                .HasDefaultValue(false);

            builder.HasMany(s => s.ClientServices)
                .WithOne(t => t.Service)
                .HasForeignKey(t => t.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.Title);
        }
    }
}

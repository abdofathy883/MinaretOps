using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class ServiceCheckpointConfig : IEntityTypeConfiguration<ServiceCheckpoint>
    {
        public void Configure(EntityTypeBuilder<ServiceCheckpoint> builder)
        {
            builder.HasKey(sc => sc.Id);

            builder.Property(sc => sc.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(sc => sc.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(sc => sc.Description)
                .HasMaxLength(1000);

            builder.Property(sc => sc.Order)
                .IsRequired();

            builder.Property(sc => sc.CreatedAt)
                .IsRequired();

            // Relationship with Service
            builder.HasOne(sc => sc.Service)
                .WithMany(s => s.ServiceCheckpoints)
                .HasForeignKey(sc => sc.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(sc => sc.ServiceId);
            builder.HasIndex(sc => new { sc.ServiceId, sc.Order });
        }
    }
}
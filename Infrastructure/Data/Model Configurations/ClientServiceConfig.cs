using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class ClientServiceConfig : IEntityTypeConfiguration<ClientService>
    {
        public void Configure(EntityTypeBuilder<ClientService> builder)
        {
            builder.HasKey(cs => cs.Id);

            builder.Property(cs => cs.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(cs => cs.ServiceCost)
                .IsRequired(false)
                .HasPrecision(18, 2);

            // Relationship with Client
            builder.HasOne(cs => cs.Client)
                .WithMany(c => c.ClientServices)
                .HasForeignKey(cs => cs.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Service
            builder.HasOne(cs => cs.Service)
                .WithMany(s => s.ClientServices)
                .HasForeignKey(cs => cs.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes for foreign keys
            builder.HasIndex(cs => cs.ClientId);
            builder.HasIndex(cs => cs.ServiceId);
        }
    }
}
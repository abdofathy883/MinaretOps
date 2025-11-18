using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class ClientServiceCheckpointConfig : IEntityTypeConfiguration<ClientServiceCheckpoint>
    {
        public void Configure(EntityTypeBuilder<ClientServiceCheckpoint> builder)
        {
            builder.HasKey(csc => csc.Id);

            builder.Property(csc => csc.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(csc => csc.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(csc => csc.CreatedAt)
                .IsRequired();

            // Relationship with ClientService
            builder.HasOne(csc => csc.ClientService)
                .WithMany(cs => cs.ClientServiceCheckpoints)
                .HasForeignKey(csc => csc.ClientServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with ServiceCheckpoint
            builder.HasOne(csc => csc.ServiceCheckpoint)
                .WithMany()
                .HasForeignKey(csc => csc.ServiceCheckpointId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Employee
            builder.HasOne(csc => csc.CompletedByEmployee)
                .WithMany()
                .HasForeignKey(csc => csc.CompletedByEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(csc => csc.ClientServiceId);
            builder.HasIndex(csc => csc.ServiceCheckpointId);
            builder.HasIndex(csc => new { csc.ClientServiceId, csc.ServiceCheckpointId })
                .IsUnique(); // Ensure one checkpoint per client-service
        }
    }
}
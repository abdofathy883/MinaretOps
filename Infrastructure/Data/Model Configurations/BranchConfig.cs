using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class BranchConfig : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Code)
                .HasMaxLength(50);

            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // One-to-one relationship with Vault
            builder.HasOne(b => b.Vault)
                .WithOne(v => v.Branch)
                .HasForeignKey<Vault>(v => v.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(b => b.Name);
            builder.HasIndex(b => b.Code)
                .IsUnique()
                .HasFilter("[Code] IS NOT NULL");
        }
    }
}

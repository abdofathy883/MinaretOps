using Core.Enums;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class VaultConfig : IEntityTypeConfiguration<Vault>
    {
        public void Configure(EntityTypeBuilder<Vault> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(v => v.VaultType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(v => v.BranchId)
                .IsRequired(false);

            builder.Property(v => v.CurrencyId)
                .IsRequired();

            builder.Property(v => v.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship with Branch (one-to-one, optional)
            builder.HasOne(v => v.Branch)
                .WithOne(b => b.Vault)
                .HasForeignKey<Vault>(v => v.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Currency
            builder.HasOne(v => v.Currency)
                .WithMany()
                .HasForeignKey(v => v.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Transactions
            builder.HasMany(v => v.Transactions)
                .WithOne(t => t.Vault)
                .HasForeignKey(t => t.VaultId)
                .OnDelete(DeleteBehavior.Restrict);

            // Note: Only one unified vault constraint is enforced at application level
            // Database constraint ensures BranchId is null when VaultType is Unified (see check constraint below)

            // Constraint: BranchId must be null when VaultType is Unified (stored as string "Unified")
            // When VaultType is Branch (stored as "Branch"), BranchId must be NOT NULL
            builder.ToTable("Vaults", t => t.HasCheckConstraint(
                "CK_Vault_UnifiedVaultConstraint",
                "([VaultType] = 'Branch' AND [BranchId] IS NOT NULL) OR ([VaultType] = 'Unified' AND [BranchId] IS NULL)"));

            // Indexes
            builder.HasIndex(v => v.BranchId);
            builder.HasIndex(v => v.CurrencyId);
            builder.HasIndex(v => v.VaultType);
        }
    }
}

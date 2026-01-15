using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class VaultTransactionConfig : IEntityTypeConfiguration<VaultTransaction>
    {
        public void Configure(EntityTypeBuilder<VaultTransaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(t => t.VaultId)
                .IsRequired();

            builder.Property(t => t.TransactionType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.CurrencyId)
                .IsRequired();

            builder.Property(t => t.TransactionDate)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.ReferenceType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.ReferenceId)
                .IsRequired(false);

            builder.Property(t => t.Notes)
                .HasMaxLength(2000);

            builder.Property(t => t.CreatedById)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship with Vault
            builder.HasOne(t => t.Vault)
                .WithMany(v => v.Transactions)
                .HasForeignKey(t => t.VaultId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Currency
            builder.HasOne(t => t.Currency)
                .WithMany()
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with ApplicationUser (CreatedBy)
            builder.HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraint: Amount must be positive
            builder.ToTable("VaultTransactions", t => t.HasCheckConstraint(
                "CK_VaultTransaction_AmountPositive",
                "[Amount] > 0"));

            // Indexes
            builder.HasIndex(t => t.VaultId);
            builder.HasIndex(t => t.CurrencyId);
            builder.HasIndex(t => t.TransactionDate);
            builder.HasIndex(t => t.TransactionType);
            builder.HasIndex(t => t.ReferenceType);
            builder.HasIndex(t => t.ReferenceId)
                .HasFilter("[ReferenceId] IS NOT NULL");
            builder.HasIndex(t => t.CreatedById);
            builder.HasIndex(t => new { t.VaultId, t.TransactionDate });
        }
    }
}

using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class ContractConfig : IEntityTypeConfiguration<CustomContract>
    {
        public void Configure(EntityTypeBuilder<CustomContract> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(c => c.ClientId)
                .IsRequired();

            builder.Property(c => c.CurrencyId)
                .IsRequired();

            builder.Property(c => c.ContractDuration)
                .IsRequired();

            builder.Property(c => c.ContractTotal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.PaidAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            // Relationship with Client
            builder.HasOne(c => c.Client)
                .WithMany()
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Currency
            builder.HasOne(c => c.Currency)
                .WithMany()
                .HasForeignKey(c => c.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add indexes
            builder.HasIndex(c => c.ClientId);
            builder.HasIndex(c => c.CurrencyId);
        }
    }
}

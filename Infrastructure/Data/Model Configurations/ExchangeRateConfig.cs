using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class ExchangeRateConfig : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.HasKey(ex => ex.Id);

            builder.Property(ex => ex.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(ex => ex.FromCurrencyId)
                .IsRequired();

            builder.Property(ex => ex.ToCurrencyId)
                .IsRequired();

            builder.Property(ex => ex.Rate)
                .IsRequired()
                .HasPrecision(18, 6);

            builder.Property(ex => ex.EffectiveFrom)
                .IsRequired();

            builder.Property(ex => ex.EffectiveTo)
                .IsRequired(false);

            builder.Property(ex => ex.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationship with Currency (FromCurrency)
            builder.HasOne(er => er.FromCurrency)
                .WithMany()
                .HasForeignKey(er => er.FromCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Currency (ToCurrency)
            builder.HasOne(er => er.ToCurrency)
                .WithMany()
                .HasForeignKey(er => er.ToCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add indexes for efficient querying
            builder.HasIndex(er => new { er.FromCurrencyId, er.ToCurrencyId });
            builder.HasIndex(er => er.EffectiveFrom);
            builder.HasIndex(er => er.IsActive);
            builder.HasIndex(er => new { er.FromCurrencyId, er.ToCurrencyId, er.IsActive, er.EffectiveFrom });

            // Check constraint to ensure FromCurrencyId != ToCurrencyId
            builder.ToTable(t => t.HasCheckConstraint("CK_ExchangeRate_DifferentCurrencies", 
                "[FromCurrencyId] != [ToCurrencyId]"));
        }
    }
}

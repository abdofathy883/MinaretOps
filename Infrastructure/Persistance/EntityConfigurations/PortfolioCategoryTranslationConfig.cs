using Core.Models.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.EntityConfigurations
{
    public class PortfolioCategoryTranslationConfig : IEntityTypeConfiguration<PortfolioCategoryTranslation>
    {
        public void Configure(EntityTypeBuilder<PortfolioCategoryTranslation> builder)
        {
            builder.HasKey(pct => pct.Id);

            builder.Property(pct => pct.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(pct => pct.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pct => pct.Description)
                .HasMaxLength(5000);

            builder.Property(pct => pct.LanguageCode)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(pct => pct.CategoryId)
                .IsRequired();

            builder.HasOne(pct => pct.PortfolioCategory)
                .WithMany(pc => pc.Translations)
                .HasForeignKey(pct => pct.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

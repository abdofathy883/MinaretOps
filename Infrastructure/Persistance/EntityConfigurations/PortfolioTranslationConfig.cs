using Core.Models.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.EntityConfigurations
{
    public class PortfolioTranslationConfig : IEntityTypeConfiguration<PortfolioTranslation>
    {
        public void Configure(EntityTypeBuilder<PortfolioTranslation> builder)
        {
            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(pt => pt.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pt => pt.Description)
                .HasMaxLength(5000);

            builder.Property(pt => pt.ImageAltText)
                .HasMaxLength(300);

            builder.Property(pt => pt.LanguageCode)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(pt => pt.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(pt => pt.PortfolioItemId)
                .IsRequired();

            builder.HasOne(pt => pt.PortfolioItem)
                .WithMany(pi => pi.Translations)
                .HasForeignKey(pt => pt.PortfolioItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

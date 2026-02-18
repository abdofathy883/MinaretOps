using Microsoft.EntityFrameworkCore;
using Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class SeoContentConfig : IEntityTypeConfiguration<SeoContent>
    {
        public void Configure(EntityTypeBuilder<SeoContent> builder)
        {
            builder.Property(x => x.Route)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Language)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(x => x.Title)
                .HasMaxLength(400);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.Keywords)
                .HasMaxLength(2000);

            builder.Property(x => x.OgTitle)
                .HasMaxLength(400);

            builder.Property(x => x.OgDescription)
                .HasMaxLength(2000);

            builder.Property(x => x.OgImage)
                .HasMaxLength(200);

            builder.Property(x => x.CanonicalUrl)
                .HasMaxLength(200);

            builder.Property(x => x.Robots)
                .HasMaxLength(100);

            // Ensure unique combination of Route and Language
            builder.HasIndex(x => new { x.Route, x.Language }).IsUnique();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class SeoContentConfig : IEntityTypeConfiguration<SeoContent>
    {
        public void Configure(EntityTypeBuilder<SeoContent> builder)
        {
            builder.Property(x => x.Route).IsRequired();
            builder.Property(x => x.Language).IsRequired().HasMaxLength(5); // en, ar, etc.
            
            // Ensure unique combination of Route and Language
            builder.HasIndex(x => new { x.Route, x.Language }).IsUnique();
        }
    }
}

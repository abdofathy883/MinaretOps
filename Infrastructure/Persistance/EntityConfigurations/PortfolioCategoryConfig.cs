using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.EntityConfigurations
{
    public class PortfolioCategoryConfig : IEntityTypeConfiguration<PortfolioCategory>
    {
        public void Configure(EntityTypeBuilder<PortfolioCategory> builder)
        {
            builder.HasKey(pc => pc.Id);

            builder.Property(pc => pc.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(pc => pc.Title)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(pc => pc.Description)
                .HasMaxLength(2000);

             builder.HasMany(pc => pc.PortfolioItems)
                .WithOne(pi => pi.PortfolioCategory)
                .HasForeignKey(pi => pi.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

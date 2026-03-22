using Core.Models.Portfolio;
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



             builder.HasMany(pc => pc.PortfolioItems)
                .WithOne(pi => pi.Category)
                .HasForeignKey(pi => pi.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

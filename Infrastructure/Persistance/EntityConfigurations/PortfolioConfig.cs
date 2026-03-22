using Core.Models.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.EntityConfigurations
{
    public class PortfolioConfig : IEntityTypeConfiguration<PortfolioItem>
    {
        public void Configure(EntityTypeBuilder<PortfolioItem> builder)
        {
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(pi => pi.ImageLink)
                .HasMaxLength(500);

            builder.HasOne(pi => pi.Category)
                .WithMany()
                .HasForeignKey(pi => pi.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class AnnouncementLinkConfig : IEntityTypeConfiguration<AnnouncementLink>
    {
        public void Configure(EntityTypeBuilder<AnnouncementLink> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(l => l.Link)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(l => l.AnnouncementId)
                .IsRequired();

            builder.HasIndex(l => l.AnnouncementId);

            builder
                .HasOne(l => l.Announcement)
                .WithMany(a => a.AnnouncementLinks)
                .HasForeignKey(l => l.AnnouncementId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

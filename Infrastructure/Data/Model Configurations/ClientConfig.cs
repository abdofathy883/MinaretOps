using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class ClientConfig : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .UseIdentityColumn(1, 1);

            // Configure properties
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.CompanyName)
                .HasMaxLength(200);

            builder.Property(c => c.PersonalPhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.CompanyNumber)
                .HasMaxLength(20);

            builder.Property(c => c.BusinessDescription)
                .IsRequired()
                .HasMaxLength(3000);

            builder.Property(c => c.DriveLink)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.DiscordChannelId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>();

            // Relationship with ClientService
            builder.HasMany(c => c.ClientServices)
                .WithOne(cs => cs.Client)
                .HasForeignKey(cs => cs.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.CompanyName);
        }
    }
}
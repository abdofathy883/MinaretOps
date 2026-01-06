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

            builder.Property(c => c.Email)
                .HasMaxLength(100);

            builder.Property(c => c.BusinessDescription)
                .IsRequired()
                .HasMaxLength(3000);

            builder.Property(c => c.DriveLink)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.BusinessActivity)
                .HasMaxLength(1000);

            builder.Property(c => c.CommercialRegisterNumber)
                .HasMaxLength(50);

            builder.Property(c => c.TaxCardNumber)
                .HasMaxLength(50);

            builder.Property(c => c.Country)
                .HasMaxLength(100);

            builder.Property(c => c.AccountManagerId)
                .IsRequired(false)
                .HasMaxLength(450);

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

            builder.HasOne(c => c.AccountManager)
                .WithMany(u => u.Clients)
                .HasForeignKey(c => c.AccountManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add indexes
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.CompanyName);
        }
    }
}
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class LeadConfig : IEntityTypeConfiguration<SalesLead>
    {
        public void Configure(EntityTypeBuilder<SalesLead> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(x => x.BusinessName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.WhatsAppNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.FollowUpReason)
                .IsRequired(false)
                .HasConversion<string>()
                .HasMaxLength(100);

            builder.Property(x => x.ContactStatus)
                .HasConversion<string>()
                .HasMaxLength(100);

            builder.Property(x => x.CurrentLeadStatus)
                .HasConversion<string>()
                .HasMaxLength(100);

            builder.Property(x => x.LeadSource)
                .HasConversion<string>()
                .HasMaxLength(100);

            builder.Property(x => x.InterestLevel)
                .HasConversion<string>()
                .HasMaxLength(100);

            builder.Property(x => x.MeetingAttend)
                .HasConversion<string>()
                .HasMaxLength(100);



            builder.Property(x => x.Notes)
                .HasMaxLength(2000);

            // Configure Relationships
            builder.HasOne(x => x.SalesRep)
                .WithMany()
                .HasForeignKey(x => x.SalesRepId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many Configuration for ServicesInterestedIn (via LeadServices join entity)
            builder.HasMany(x => x.ServicesInterestedIn)
                .WithOne(x => x.Lead)
                .HasForeignKey(x => x.LeadId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => x.SalesRepId);
            builder.HasIndex(x => x.CreatedById);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}

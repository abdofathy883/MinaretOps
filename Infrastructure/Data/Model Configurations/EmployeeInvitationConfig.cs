using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class EmployeeInvitationConfig : IEntityTypeConfiguration<EmployeeOnBoardingInvitation>
    {
        public void Configure(EntityTypeBuilder<EmployeeOnBoardingInvitation> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(i => i.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Role)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(i => i.InvitationToken)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(i => i.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(i => i.InvitedByUserId)
                .IsRequired()
                .HasMaxLength(450); // Match AspNetUsers.Id length

            // Nullable string properties with max lengths
            builder.Property(i => i.FirstName)
                .HasMaxLength(100);

            builder.Property(i => i.LastName)
                .HasMaxLength(100);

            builder.Property(i => i.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(i => i.City)
                .HasMaxLength(100);

            builder.Property(i => i.Street)
                .HasMaxLength(200);

            builder.Property(i => i.NID)
                .HasMaxLength(14); // Egyptian National ID is 14 digits

            builder.Property(i => i.PaymentNumber)
                .HasMaxLength(50);

            builder.Property(i => i.Password)
                .HasMaxLength(50);

            // Foreign key relationship
            builder.HasOne(i => i.InvitedBy)
                .WithMany()
                .HasForeignKey(i => i.InvitedByUserId)
                .OnDelete(DeleteBehavior.Restrict); // Changed from SetNull since InvitedByUserId is required
        }
    }
}
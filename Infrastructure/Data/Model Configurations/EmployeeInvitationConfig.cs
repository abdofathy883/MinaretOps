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
            builder.HasIndex(i => i.InvitationToken).IsUnique();
            builder.HasIndex(i => i.Email);
            builder.Property(i => i.Email).IsRequired().HasMaxLength(100);
            builder.Property(i => i.InvitationToken).IsRequired().HasMaxLength(500);
            builder.HasOne(i => i.InvitedBy)
                .WithMany()
                .HasForeignKey(i => i.InvitedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
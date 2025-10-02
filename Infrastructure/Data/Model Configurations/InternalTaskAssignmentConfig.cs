using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class InternalTaskAssignmentConfig : IEntityTypeConfiguration<InternalTaskAssignment>
    {
        public void Configure(EntityTypeBuilder<InternalTaskAssignment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(a => a.UserId)
                .IsRequired(false);

            builder.Property(a => a.IsLeader)
                .IsRequired();

            builder.HasOne(a => a.User)
                .WithMany(u => u.InternalTaskAssignments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.InternalTaskId);
        }
    }
}



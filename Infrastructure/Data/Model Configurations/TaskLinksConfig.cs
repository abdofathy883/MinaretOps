using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class TaskLinksConfig : IEntityTypeConfiguration<TaskCompletionResources>
    {
        public void Configure(EntityTypeBuilder<TaskCompletionResources> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(r => r.URL)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(r => r.TaskId)
                .IsRequired();
        }
    }
}

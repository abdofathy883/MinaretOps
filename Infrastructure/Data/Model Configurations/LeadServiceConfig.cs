using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class LeadServiceConfig : IEntityTypeConfiguration<LeadServices>
    {
        public void Configure(EntityTypeBuilder<LeadServices> builder)
        {
            builder.HasKey(ls => ls.Id);

            builder.Property(ls => ls.Id)
                .UseIdentityColumn(1, 1);

            builder.HasOne(ls => ls.Service)
                   .WithMany(s => s.LeadServices)
                   .HasForeignKey(ls => ls.ServiceId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ls => ls.Lead)
                     .WithMany(l => l.ServicesInterestedIn)
                     .HasForeignKey(ls => ls.LeadId)
                     .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

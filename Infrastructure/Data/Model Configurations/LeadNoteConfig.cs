using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Model_Configurations
{
    public class LeadNoteConfig : IEntityTypeConfiguration<LeadNote>
    {
        public void Configure(EntityTypeBuilder<LeadNote> builder)
        {
            builder.Property(n => n.Note)
                .IsRequired()
                .HasMaxLength(6000);

            builder.Property(n => n.CreatedById)
                .IsRequired();

            builder.Property(n => n.LeadId)
                .IsRequired();

            builder.HasOne(n => n.CreatedBy)
                .WithMany()
                .HasForeignKey(n => n.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(n => n.Lead)
                .WithMany(l => l.Notes)
                .HasForeignKey(n => n.LeadId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

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
    internal class OutboxConfig : IEntityTypeConfiguration<Outbox>
    {
        public void Configure(EntityTypeBuilder<Outbox> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(o => o.OpTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.PayLoad)
            .IsRequired();

            builder.Property(o => o.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(o => o.ProcessedAt); // speeds up lookup for unprocessed rows
        }
    }
}

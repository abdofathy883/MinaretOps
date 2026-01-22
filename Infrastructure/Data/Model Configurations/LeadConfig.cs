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
    public class LeadConfig : IEntityTypeConfiguration<SalesLead>
    {
        public void Configure(EntityTypeBuilder<SalesLead> builder)
        {
            throw new NotImplementedException();
        }
    }
}

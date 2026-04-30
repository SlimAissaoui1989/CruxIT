using CruxIT.Library.DataToolkit.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.EntityFramework.Configurations
{
    public class TraceConfiguration : IEntityTypeConfiguration<Trace>
    {
        public void Configure(EntityTypeBuilder<Trace> builder)
        {
            builder.HasIndex(i => new
            {
                i.TableName,
                i.TableId,
            });
        }
    }
}

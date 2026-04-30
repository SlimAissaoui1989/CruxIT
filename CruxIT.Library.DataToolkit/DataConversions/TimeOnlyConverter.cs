using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.DataConversions
{
    public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
    {
        public TimeOnlyConverter() : base(
            d => d.ToTimeSpan(),
            d => TimeOnly.FromTimeSpan(d))
        { }
    }
}

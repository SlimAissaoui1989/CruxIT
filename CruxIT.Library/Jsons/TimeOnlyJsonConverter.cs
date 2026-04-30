using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CruxIT.Library.Jsons
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        //private const string format1 = "hh:mm";
        private const string format = "hh:mm:ss";
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeOnly.Parse(reader.GetString() ?? TimeOnly.MinValue.ToString());
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(format, CultureInfo.InvariantCulture));
        }
    }
}

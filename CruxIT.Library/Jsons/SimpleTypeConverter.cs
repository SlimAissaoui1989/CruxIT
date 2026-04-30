using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CruxIT.Library.Jsons
{
    public class SimpleTypeConverter<T> : JsonConverter<T>
    {
        private readonly List<Type> SimpleTypes = new List<Type>
    {
        typeof(string), typeof(int), typeof(decimal), typeof(double), typeof(float), typeof(char),
        typeof(bool), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
        typeof(uint), typeof(long), typeof(ulong), typeof(DateTime)
    };

        private bool IsSimpleType(Type type)
        {
            return SimpleTypes.Contains(type) || type.IsArray && IsSimpleType(type.GetElementType()!)
                   || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                       && IsSimpleType(Nullable.GetUnderlyingType(type)!));
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var innerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = null,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(value, innerOptions);
            T? valueCloned = JsonSerializer.Deserialize<T>(json);
            if (valueCloned != null)
            {
                IEnumerable<PropertyInfo> propertyInfos = valueCloned.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (!IsSimpleType(propertyInfo.PropertyType))
                    {
                        propertyInfo.SetValue(valueCloned, CxObjectHelper.GetDefault(propertyInfo.PropertyType));
                    }
                }
                JsonSerializer.Serialize(writer, valueCloned);
            }
            else
                JsonSerializer.Serialize(writer, json);
        }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException(); // This example focuses only on serialization.
        }
    }
}

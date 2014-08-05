using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Core
{
    internal static class ExtensionMethods
    {
        public static void WriteOptionalProperty(this JsonWriter writer, string propertyName, object value)
        {
            if (value == null || (value is string && string.IsNullOrEmpty((string)value))) { return; }

            writer.WritePropertyName(propertyName);
            writer.WriteValue(value);
        }

        public static void WriteOptionalRef(this JsonWriter writer, string propertyName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.WritePropertyName(propertyName);
                writer.WriteStartObject();
                writer.WritePropertyName("$ref");
                writer.WriteValue(value);
                writer.WriteEnd();
            }
        }

        public static void WriteOptionalList(this JsonWriter writer, string propertyName, IEnumerable<string> values)
        {
            if (values != null && values.Count() > 0)
            {
                writer.WritePropertyName(propertyName);
                writer.WriteStartArray();

                values.ToList().ForEach(v => writer.WriteValue(v));

                writer.WriteEnd();
            }
        }
    }
}

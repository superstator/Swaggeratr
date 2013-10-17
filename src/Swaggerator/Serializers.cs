using Newtonsoft.Json;
using Swaggerator.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Swaggerator
{
	internal static class Serializers
	{
		internal static string WriteApi(Uri basePath, string servicePath, Type serviceType, Stack<Type> typeStack)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.WriteStartObject();
				writer.WritePropertyName("apiVersion");
				writer.WriteValue(serviceType.Assembly.GetName().Version.ToString());
				writer.WritePropertyName("swaggerVersion");
				writer.WriteValue(Globals.SWAGGER_VERSION);
				writer.WritePropertyName("basePath");
				writer.WriteValue(string.Format("{0}://{1}", basePath.Scheme, basePath.Authority));
				writer.WritePropertyName("resourcePath");
				writer.WriteValue(string.Format(servicePath));

				writer.WritePropertyName("apis");
				writer.WriteRawValue(WriteApis(Mappers.FindMethods(servicePath, serviceType, typeStack)));

				writer.WritePropertyName("models");
				writer.WriteRawValue(WriteModels(typeStack));
			}
			return sb.ToString();
		}

		private static string WriteApis(IEnumerable<Models.Method> methods)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.WriteStartArray();
				foreach (Models.Method m in methods.OrderBy(s => s.path))
				{
					writer.WriteRawValue(JsonConvert.SerializeObject(m));
				}
				writer.WriteEnd();
			}
			return sb.ToString();
		}

		internal static string WriteModels(Stack<Type> typeStack)
		{
			StringBuilder sb = new StringBuilder();

			sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.None;
				writer.WriteStartObject();
				while (typeStack.Count > 0)
				{
					Type t = typeStack.Pop();
					if (t.GetCustomAttribute<HiddenAttribute>() != null) { continue; }
					writer.WritePropertyName(t.FullName);
					writer.WriteRawValue(WriteType(t, typeStack));
				}
				writer.WriteEnd();
			}

			return sb.ToString();
		}

		internal static string WriteType(Type t, Stack<Type> typeStack)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.WriteStartObject();
				writer.WritePropertyName("id");
				writer.WriteValue(t.FullName);
				writer.WritePropertyName("properties");
				writer.WriteRawValue(WriteProperties(t, typeStack));
			}
			return sb.ToString();
		}

		private static string WriteProperties(Type t, Stack<Type> typeStack)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.WriteStartObject();
				foreach (PropertyInfo pi in t.GetProperties())
				{
					if (pi.GetCustomAttribute<DataMemberAttribute>() == null ||
						 pi.GetCustomAttribute<HiddenAttribute>() != null) { continue; }
					writer.WritePropertyName(pi.Name);
					writer.WriteRawValue(WriteProperty(pi, typeStack));
				}
				writer.WriteEnd();
			}
			return sb.ToString();
		}

		private static string WriteProperty(PropertyInfo pi, Stack<Type> typeStack)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.WriteStartObject();
				writer.WritePropertyName("type");
				writer.WriteValue(Helpers.MapSwaggerType(pi.PropertyType, typeStack));
				writer.WritePropertyName("required");
				writer.WriteValue(true);

				if (Helpers.MapSwaggerType(pi.PropertyType, typeStack) == "array")
				{
					writer.WritePropertyName("items");
					writer.WriteStartObject();
					writer.WritePropertyName("$ref");
					writer.WriteValue(Helpers.MapElementType(pi.PropertyType, typeStack));
				}

				DescriptionAttribute description = pi.GetCustomAttribute<DescriptionAttribute>();
				if (description != null)
				{
					writer.WritePropertyName("description");
					writer.WriteValue(description.Description);
				}
				writer.WriteEnd();
			}
			return sb.ToString();
		}
	}
}

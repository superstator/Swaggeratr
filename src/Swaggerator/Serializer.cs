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
	internal class Serializer
	{
		internal Serializer(Dictionary<string, Configuration.TagElement> tagSettings)
		{
			_TagSettings = tagSettings;
			_Mapper = new Mapper(tagSettings);
		}

		private readonly Dictionary<string, Configuration.TagElement> _TagSettings;
		private readonly Mapper _Mapper;

		internal string WriteApi(Uri basePath, string servicePath, Type serviceType, Stack<Type> typeStack)
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
				writer.WriteRawValue(WriteApis(_Mapper.FindMethods(servicePath, serviceType, typeStack)));

				writer.WritePropertyName("models");
				writer.WriteRawValue(WriteModels(typeStack));
			}
			return sb.ToString();
		}

		private string WriteApis(IEnumerable<Models.Method> methods)
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

		internal string WriteModels(Stack<Type> typeStack)
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

		internal string WriteType(Type t, Stack<Type> typeStack)
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

		private string WriteProperties(Type t, Stack<Type> typeStack)
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

		private string WriteProperty(PropertyInfo pi, Stack<Type> typeStack)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);

			Type pType = pi.PropertyType;
			bool required = true;
			if (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(System.Nullable<>))
			{
				required = false;
				pType = pType.GetGenericArguments().First();
			}

			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.WriteStartObject();
				writer.WritePropertyName("type");
				writer.WriteValue(Helpers.MapSwaggerType(pType, typeStack));
				writer.WritePropertyName("required");
				writer.WriteValue(required);

				if (Helpers.MapSwaggerType(pType, typeStack) == "array")
				{
					writer.WritePropertyName("items");
					writer.WriteStartObject();
					writer.WritePropertyName("$ref");
					writer.WriteValue(Helpers.MapElementType(pType, typeStack));
				}

				if (pType.IsEnum)
				{
					writer.WritePropertyName("enum");
					writer.WriteStartArray();
					foreach (string value in pType.GetEnumNames())
					{
						writer.WriteValue(value);
					}
					writer.WriteEndArray();
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

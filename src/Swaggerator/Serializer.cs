/*
 * Copyright (c) 2014 Digimarc Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * Serializer.cs : Methods to produce JSON documentation via both automatic model serialization and manual JSONWriter calls.
 */


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

namespace Swaggerator
{
	internal class Serializer
	{
		internal Serializer(IEnumerable<string> hiddenTags)
		{
			HiddenTags = hiddenTags ?? new List<string>();
			_Mapper = new Mapper(HiddenTags);
		}

		private readonly IEnumerable<string> HiddenTags;
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
					if (t.GetCustomAttributes<TagAttribute>().Select(tn => tn.TagName).Any(HiddenTags.Contains)) { continue; }

					var propName = Helpers.GetDataContractNamePropertyValue(t) ?? t.FullName;
					writer.WritePropertyName(propName);
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

				var idValue = Helpers.GetDataContractNamePropertyValue(t) ?? t.FullName;
				writer.WritePropertyName("id");
				writer.WriteValue(idValue);

				writer.WritePropertyName("properties");
				writer.WriteRawValue(WriteProperties(t, typeStack));
			}
			return sb.ToString();
		}

		private string WriteProperties(Type type, Stack<Type> typeStack)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);

			//var hasParent = type.BaseType != typeof (Object);
			
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.WriteStartObject();

				var properties = type.GetProperties();

				//only pass immediate class properties at a time to write properties in the order of inheritance from base. (i.e. base first, derived next.) 

				//get a stack of class types within the passed type so that the base class comes at the top.
				var classStack = new Stack<Type>();
				foreach (var propertyInfo in properties.Where(propertyInfo => !classStack.Contains(propertyInfo.DeclaringType)))
				{
					classStack.Push(propertyInfo.DeclaringType);
				}
				//iterate through each class to only get properties for that class to write
				foreach (var propertiesToWrite in classStack.Select(cType => properties.Where(p => p.DeclaringType == cType)))
				{
					WritePropertyNameValuePair(propertiesToWrite, typeStack, writer);
				}

				writer.WriteEnd();
			}
			return sb.ToString();
		}


		private void WritePropertyNameValuePair(IEnumerable<PropertyInfo> properties, Stack<Type> typeStack, JsonWriter writer)
		{
			foreach (var pi in properties)
			{
				if (pi.GetCustomAttribute<DataMemberAttribute>() == null 
					|| pi.GetCustomAttribute<HiddenAttribute>() != null 
					|| pi.GetCustomAttributes<TagAttribute>().Select(t => t.TagName).Any(HiddenTags.Contains))
					continue;

				var dataMemberAttribute = pi.GetCustomAttribute<DataMemberAttribute>();
				var propertyName = pi.Name;
				if (dataMemberAttribute != null && !string.IsNullOrEmpty(dataMemberAttribute.Name))
					propertyName = dataMemberAttribute.Name;

				writer.WritePropertyName(propertyName);
				writer.WriteRawValue(WriteProperty(pi, typeStack));
			}
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
				var memberProperties = pi.GetCustomAttribute<MemberPropertiesAttribute>();

				var typeValue = memberProperties != null ? Helpers.MapSwaggerType(pType, typeStack, memberProperties.TypeSizeNote) : Helpers.MapSwaggerType(pType, typeStack);

				//If the Name property in DataContract is defined for this property type, use that name instead.
				//Needed for cases where a DataContract uses another DataContract as a return type for a member. 
				var dcName = Helpers.GetDataContractNamePropertyValue(pType);
				
				if(!string.IsNullOrEmpty(dcName))
				{
					typeValue = dcName;
					if (memberProperties != null && !string.IsNullOrEmpty(memberProperties.TypeSizeNote))
						typeValue = string.Format("{0}({1})", dcName, memberProperties.TypeSizeNote);
				}


				writer.WriteStartObject();

				writer.WritePropertyName("type");
				writer.WriteValue(typeValue);

				writer.WritePropertyName("required");
				writer.WriteValue(required);

				DescriptionAttribute description = pi.GetCustomAttribute<DescriptionAttribute>();
				if (description != null)
				{
					writer.WritePropertyName("description");
					writer.WriteValue(description.Description);
				}
				else
				{
					if (memberProperties != null)
					{
						writer.WritePropertyName("description");
						writer.WriteValue(memberProperties.Description);
					}
				}

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

				writer.WriteEnd();
			}
			return sb.ToString();
		}
	}
}

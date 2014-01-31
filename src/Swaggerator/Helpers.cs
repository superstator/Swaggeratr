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
 * Helpers.cs : Miscellaneous helper methods for reading type names, attribute values, etc.
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Swaggerator
{
	internal static class Helpers
	{
		private static readonly Regex _GenericTypeRegex = new Regex("^(.*)`.*");


		public static string MapSwaggerType(Type type, Stack<Type> typeMap, string typeNote = null)
		{
			//built-in types
			if (type == typeof (bool)) {return BuildTypeString("boolean", typeNote);}
			if (type == typeof(byte)) { return BuildTypeString("integer", "8", typeNote); }
			if (type == typeof (sbyte)) { return BuildTypeString("integer", "8, signed", typeNote);}
			if (type == typeof (char)) { return BuildTypeString("character", typeNote); }
			if (type == typeof(decimal)) { return BuildTypeString("decimal", typeNote); }
			if (type == typeof(double)) { return BuildTypeString("double", typeNote); }
			if (type == typeof(float)) { return BuildTypeString("float", typeNote); }
			if (type == typeof(int)) { return BuildTypeString("integer", "32", typeNote); }
			if (type == typeof(uint)) { return BuildTypeString("integer", "32, unsigned", typeNote); }
			if (type == typeof(long)) { return BuildTypeString("integer", "64", typeNote); }
			if (type == typeof(ulong)) { return BuildTypeString("integer", "64, unsigned", typeNote); }
			if (type == typeof(short)) { return BuildTypeString("integer", "16", typeNote); }
			if (type == typeof(ushort)) { return BuildTypeString("integer", "16, unsigned", typeNote); }
			if (type == typeof(string)) { return BuildTypeString("string", typeNote); }
			if (type == typeof(DateTime)) { return BuildTypeString("Date", typeNote); }
			if (type == typeof (Guid)) { return BuildTypeString("string", typeNote);}
			if (type == typeof(Stream)) { return BuildTypeString("stream", typeNote); }

			if (type == typeof (void)) {return "None";}

			//it's an enum, use string as the property type and enum values will be serialized later
			if (type.IsEnum) { return "string"; }

			//it's a collection/array, so it will use the swagger "container" syntax
			if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
			{
				return "array";
			}

			//it's a complex type, so we'll need to map it later
			if (!typeMap.Contains(type)) { typeMap.Push(type); }
			return type.FullName;
		}

		private static string BuildTypeString(string typeName, string defaultNote = null, string typeNote = null)
		{
			const string resultFormat = "{0}({1})";
				
			if (string.IsNullOrEmpty(defaultNote) && string.IsNullOrEmpty(typeNote))
				return typeName;

			return string.IsNullOrEmpty(typeNote) 
				? string.Format(resultFormat, typeName, defaultNote)
				: string.Format(resultFormat, typeName, typeNote);
		}
		
		public static T1 GetCustomAttributeValue<T1, T2>(MethodInfo method, string propertyName)
			where T1 : class
			where T2 : Attribute
		{
			T2 attr = method.GetCustomAttribute<T2>();
			if (attr == null) { return null; }

			PropertyInfo prop = typeof(T2).GetProperty(propertyName);
			if (prop == null || prop.PropertyType != typeof(T1)) { return null; }

			return prop.GetValue(attr) as T1;
		}

		internal static string MapElementType(Type type, Stack<Type> typeStack)
		{
			Type enumType = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if (enumType == null) { throw new ArgumentException("Type must be an IEnumerable<T>."); }

			Type elementType = enumType.GetGenericArguments().First();

			var result = MapSwaggerType(elementType, typeStack);
			var dataContractNameOfElementType = GetDataContractNamePropertyValue(elementType);
			result = string.IsNullOrEmpty(dataContractNameOfElementType) ? result : dataContractNameOfElementType;

			return result;
		}

		internal static bool TagIsHidden(this Dictionary<string, Configuration.TagElement> tagConfigurations, IEnumerable<string> itemTags)
		{
			return tagConfigurations.Values.Any(t => t.Visibile.Equals(false) && itemTags.Contains(t.Name));
		}

		internal static string GetDataContractNamePropertyValue(Type type)
		{
			var attrib = type.GetCustomAttribute<DataContractAttribute>();
			if (attrib != null && !string.IsNullOrEmpty(attrib.Name))
				return attrib.Name;

			return null;
		}
	}
}

/*
 * Copyright (c) 2013 Digimarc Corporation
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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Swaggerator
{
	internal static class Helpers
	{
		private static readonly Regex _GenericTypeRegex = new Regex("^(.*)`.*");


		public static string MapSwaggerType(Type type, Stack<Type> typeMap)
		{
			if (type == typeof(byte)) { return "byte"; }
			if (type == typeof(bool)) { return "boolean"; }
			if (type == typeof(int)) { return "integer"; }
			if (type == typeof(long)) { return "long"; }
			if (type == typeof(float)) { return "float"; }
			if (type == typeof(double)) { return "double"; }
			if (type == typeof(string)) { return "string"; }
			if (type == typeof(DateTime)) { return "Date"; }

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
			return MapSwaggerType(elementType, typeStack);
		}

		internal static bool TagIsHidden(this Dictionary<string, Configuration.TagElement> tagConfigurations, IEnumerable<string> itemTags)
		{
			return tagConfigurations.Values.Any(t => t.Visibile.Equals(false) && itemTags.Contains(t.Name));
		}
	}
}

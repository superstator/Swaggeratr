using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

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

		//public static string GetTypeName(Type type, Stack<Type> typeMap)
		//{
		//	if (type.IsArray)
		//	{
		//		string elementType = MapSwaggerType(type.GetElementType(), typeMap);
		//		return elementType + "[]";
		//	}

		//	if (type.IsGenericType)
		//	{
		//		string genericArgs = string.Join(", ", type.GenericTypeArguments.Select(t => MapSwaggerType(t, typeMap)));
		//		return _GenericTypeRegex.Replace(type.FullName, string.Format("$1<{0}>", genericArgs));
		//	}

		//	return type.FullName;
		//}

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
	}
}

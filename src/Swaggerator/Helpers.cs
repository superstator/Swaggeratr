using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator
{
	internal static class Helpers
	{
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
			//if (type.IsArray) { return MapSwaggerArrayType(type, typeMap); }
			else
			{
				if (!typeMap.Contains(type)) { typeMap.Push(type); }
				return type.FullName;
			}
		}

		//public static string MapSwaggerArrayType(Type type, Stack<Type> typeMap)
		//{
		//	return @"{""type"":""array"",""items"":""string""}";
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
	}
}

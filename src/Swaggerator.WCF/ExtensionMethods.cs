using Swaggerator.Attributes;
using Swaggerator.Core.Models.APIs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.WCF
{
    internal static class ExtensionMethods
    {
        public static T1 GetCustomAttributeValue<T1, T2>(this MemberInfo method, string propertyName)
            where T1 : class
            where T2 : Attribute
        {
            T2 attr = method.GetCustomAttribute<T2>();
            if (attr == null) { return null; }

            PropertyInfo prop = typeof(T2).GetProperty(propertyName);
            if (prop == null || prop.PropertyType != typeof(T1)) { return null; }

            return prop.GetValue(attr) as T1;
        }

        public static string ToSwaggerType(this Type type, Stack<Type> typeMap, string typeNote = null)
        {
            //built-in types
            if (type == typeof(bool)) { return BuildTypeString("boolean", typeNote); }
            if (type == typeof(byte)) { return BuildTypeString("integer", "8", typeNote); }
            if (type == typeof(sbyte)) { return BuildTypeString("integer", "8, signed", typeNote); }
            if (type == typeof(char)) { return BuildTypeString("character", typeNote); }
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
            if (type == typeof(Guid)) { return BuildTypeString("string", typeNote); }
            if (type == typeof(Stream)) { return BuildTypeString("stream", typeNote); }

            if (type == typeof(void)) { return "None"; }

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

        public static string ToElementType(this Type type, Stack<Type> typeStack)
        {
            Type enumType = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumType == null) { throw new ArgumentException("Type must be an IEnumerable<T>."); }

            Type elementType = enumType.GetGenericArguments().First();

            var result = elementType.ToSwaggerType(typeStack);
            var dataContractNameOfElementType = elementType.GetCustomAttributeValue<string, DataContractAttribute>("Name");
            result = string.IsNullOrEmpty(dataContractNameOfElementType) ? result : dataContractNameOfElementType;

            return result;
        }

        public static IEnumerable<APIResponseCode> GetResponseCodes(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<ResponseCodeAttribute>().Select(rca => new APIResponseCode
            {
                code = rca.Code,
                message = rca.Description
            }).OrderBy(c => c.code);
        }
    }
}

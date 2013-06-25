using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Swaggerator.Models;
using Swaggerator.Attributes;
using System.Runtime.Serialization;

namespace Swaggerator
{
    internal class Mappers
    {
        /// <summary>
        /// Find methods of the supplied type which have WebGet or WebInvoke attributes.
        /// </summary>
        /// <param name="serviceType">The implementation type to search.</param>
        internal static IEnumerable<Method> FindMethods(Type serviceType, Stack<Type> typeStack)
        {
            List<Tuple<string, Operation>> operations = new List<Tuple<string, Operation>>();

            //search all interfaces for this type for potential DataContracts, and build a set of operations
            Type[] interfaces = serviceType.GetInterfaces();
            foreach (Type i in interfaces)
            {
                Attribute dc = i.GetCustomAttribute(typeof(ServiceContractAttribute));
                if (dc != null)
                {
                    //found a DataContract, now get a service map and inspect the methods for WebGet/WebInvoke
                    InterfaceMapping map = serviceType.GetInterfaceMap(i);
                    operations.AddRange(GetOperations(map, typeStack));
                }
            }

            List<Method> methods = new List<Method>();

            //go through the discovered Operations, and combine any like Uri's into Methods.
            foreach (Tuple<string, Operation> t in operations)
            {
                Method method = (from m in methods
                                 where m.path.Equals(t.Item1)
                                 select m).FirstOrDefault();
                if (method == null)
                {
                    method = new Method() { path = t.Item1 };
                    methods.Add(method);
                }
                method.operations.Add(t.Item2);
            }

            return methods;
        }

        private static IEnumerable<Tuple<string, Operation>> GetOperations(InterfaceMapping map, Stack<Type> typeStack)
        {
            for (int index = 0; index < map.InterfaceMethods.Count(); index++)
            {
                MethodInfo implementation = map.TargetMethods[index];
                MethodInfo declaration = map.InterfaceMethods[index];

                //if the method is marked Hidden anywhere, skip it
                if (implementation.GetCustomAttribute<Attributes.HiddenAttribute>() != null ||
                    declaration.GetCustomAttribute<Attributes.HiddenAttribute>() != null) { continue; }

                //find the WebGet/Invoke attributes, or skip if neither is present
                WebGetAttribute wg = declaration.GetCustomAttribute<WebGetAttribute>();
                WebInvokeAttribute wi = declaration.GetCustomAttribute<WebInvokeAttribute>();
                if (wg == null && wi == null) { continue; }

                string httpMethod = (wi == null) ? "GET" : wi.Method;
                string uriTemplate = (wi == null) ? wg.UriTemplate ?? "" : wi.UriTemplate ?? "";

                //implementation description overrides interface description
                string description =
                    Helpers.GetCustomAttributeValue<string, DescriptionAttribute>(implementation, "Description") ??
                    Helpers.GetCustomAttributeValue<string, DescriptionAttribute>(declaration, "Description") ??
                    "";

                Operation operation = new Operation()
                {
                    httpMethod = httpMethod,
                    nickname = declaration.Name + httpMethod,
                    responseClass = Helpers.MapSwaggerType(declaration.ReturnType, typeStack),
                    //TODO add mechanism to make this somewhat configurable
                    summary = (description.Length > 0) ? description.Substring(0, description.IndexOf(".")) : "",
                    notes = description
                };

                ParameterInfo[] parameters = declaration.GetParameters();
                foreach (ParameterInfo parameter in parameters)
                {
                    Parameter parm = new Parameter();
                    parm.name = parameter.Name;
                    parm.allowMultiple = false;
                    parm.required = true;
                    parm.dataType = Helpers.MapSwaggerType(parameter.ParameterType, typeStack);
                    if (uriTemplate.Contains("{" + parameter.Name + "}")) // need better test for query, etc.
                    {
                        parm.paramType = "path";
                    }
                    else
                    {
                        parm.paramType = "body";
                    }
                    operation.parameters.Add(parm);
                }

                yield return new Tuple<string, Operation>(uriTemplate, operation);
            }
        }
    }
}

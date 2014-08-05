using Swaggerator.Attributes;
using Swaggerator.Core.Models.Services;
using Swaggerator.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Swaggerator.Core.Models.APIs;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Runtime.Serialization;

namespace Swaggerator.WCF.Reflector
{
    public class ServiceReflector : Swaggerator.WCF.Reflector.IServiceReflector
    {
        public ServiceReflector() : this(null) { }

        public ServiceReflector(IEnumerable<string> hiddenTags)
        {
            _HiddenTags = hiddenTags ?? new List<string>();
            _MarkTagged = false;
        }

        IEnumerable<string> _HiddenTags;
        bool _MarkTagged;

        public ServiceList GetServices()
        {
            return GetServices(AppDomain.CurrentDomain);
        }

        public ServiceList GetServices(AppDomain searchDomain)
        {
            var serviceList = new ServiceList
            {
                swaggerVersion = Globals.SWAGGER_VERSION,
                apiVersion = "No Swaggerized assemblies."
            };

            Assembly[] searchAssemblies = searchDomain.GetAssemblies();

            bool foundAssembly = false;
            foreach (Assembly assm in searchAssemblies)
            {
                IEnumerable<Service> services = GetDiscoveratedServices(assm);
                if (services.Any())
                {
                    if (!foundAssembly)
                    {
                        foundAssembly = true;
                        serviceList.apiVersion = assm.GetName().Version.ToString();
                    }
                    else
                    {
                        //if we've already found services in other assemblies, go to generic "multiple assemblies" message
                        serviceList.apiVersion = "Multiple Assemblies";
                    }

                    serviceList.apis.AddRange(services);
                }
            }

            return serviceList;
        }

        private IEnumerable<Service> GetDiscoveratedServices(Assembly assembly)
        {
            IEnumerable<TypeInfo> types;
            try
            {
                types = assembly.DefinedTypes;
            }
            catch (ReflectionTypeLoadException)
            {
                //couldn't load this assembly - probably a non-issue
                yield break;
            }

            foreach (TypeInfo ti in types)
            {
                SwaggeratedAttribute da = ti.GetCustomAttribute<SwaggeratedAttribute>();
                if (da != null)
                {
                    DescriptionAttribute descAttr = ti.GetCustomAttribute<DescriptionAttribute>();
                    var service = new Service
                    {
                        path = da.LocalPath,
                        description = (descAttr == null) ? da.Description : descAttr.Description
                    };
                    yield return service;
                }
            }
        }

        public Type FindServiceTypeByPath(string servicePath)
        {
            //TODO - further refactoring for testability
            Assembly[] allAssm = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in allAssm)
            {
                foreach (TypeInfo ti in assembly.DefinedTypes)
                {
                    SwaggeratedAttribute da = ti.GetCustomAttribute<SwaggeratedAttribute>();
                    if (da != null && da.LocalPath == servicePath)
                    {
                        return ti.AsType();
                    }
                }
            }
            return null;
        }


        public API GetAPIDetails(Uri baseUri, string servicePath)
        {
            Type serviceType = FindServiceTypeByPath(string.Format("/{0}", servicePath));

            Stack<Type> typeStack = new Stack<Type>();

            var api = new API
            {
                apiVersion = serviceType.Assembly.GetName().Version.ToString(),
                swaggerVersion = Globals.SWAGGER_VERSION,
                basePath = string.Format("{0}://{1}", baseUri.Scheme, baseUri.Authority),
                resourcePath = string.Format(servicePath)
            };

            api.apis = FindAPIResources(servicePath, serviceType, typeStack);
            api.models = FindAPIModels(typeStack);

            return api;
        }

        /// <summary>
        /// Find methods of the supplied type which have WebGet or WebInvoke attributes.
        /// </summary>
        /// <param name="path">Base service path.</param>
        /// <param name="serviceType">The implementation type to search.</param>
        /// <param name="typeStack">Types to be documented in the models section.</param>
        private List<Core.Models.APIs.APIResource> FindAPIResources(string servicePath, Type serviceType, Stack<Type> typeStack)
        {
            var operations = new List<Tuple<string, APIOperation>>();

            //search all interfaces for this type for potential DataContracts, and build a set of operations
            var interfaces = serviceType.GetInterfaces();
            foreach (Type i in interfaces)
            {
                Attribute dc = i.GetCustomAttribute(typeof(ServiceContractAttribute));
                if (dc != null)
                {
                    //found a DataContract, now get a service map and inspect the methods/resources for WebGet/WebInvoke
                    InterfaceMapping map = serviceType.GetInterfaceMap(i);
                    operations.AddRange(FindOperations(map, typeStack));
                }
            }

            var resources = new List<APIResource>();

            //go through the discovered Operations, and combine any like Uri's into APIResources.
            foreach (Tuple<string, APIOperation> t in operations)
            {
                APIResource resource = (from m in resources
                                        where m.path.Equals(t.Item1)
                                        select m).FirstOrDefault();
                if (resource == null)
                {
                    resource = new APIResource { path = servicePath + t.Item1 };
                    resources.Add(resource);
                }
                resource.operations.Add(t.Item2);
            }

            return resources;
        }

        /// <summary>
        /// Constructs individual operation objects based on the service implementation.
        /// </summary>
        /// <param name="map">Mapping of the service interface & implementation.</param>
        /// <param name="typeStack">Complex types that will need later processing.</param>
        private IEnumerable<Tuple<string, APIOperation>> FindOperations(InterfaceMapping map, Stack<Type> typeStack)
        {
            //TODO - This is probably the worst block of code in here now. Break down into methods, probably split into DI-able classes like serializers

            for (int index = 0; index < map.InterfaceMethods.Count(); index++)
            {
                MethodInfo implementation = map.TargetMethods[index];
                MethodInfo declaration = map.InterfaceMethods[index];

                //if the method is marked Hidden anywhere, skip it
                if (implementation.GetCustomAttribute<HiddenAttribute>() != null ||
                      declaration.GetCustomAttribute<HiddenAttribute>() != null) { continue; }

                //if a tag from either implementation or declaration is marked as not visible, skip it
                var methodTags = implementation.GetCustomAttributes<TagAttribute>().Select(t => t.TagName).Concat(declaration.GetCustomAttributes<TagAttribute>().Select(t => t.TagName));
                if (methodTags.Any(_HiddenTags.Contains)) { continue; }

                //find the WebGet/Invoke attributes, or skip if neither is present
                WebGetAttribute wg = declaration.GetCustomAttribute<WebGetAttribute>();
                WebInvokeAttribute wi = declaration.GetCustomAttribute<WebInvokeAttribute>();
                if (wg == null && wi == null) { continue; }

                string httpMethod = (wi == null) ? "GET" : wi.Method;

                string uriTemplate = (wi == null) ? wg.UriTemplate ?? "" : wi.UriTemplate ?? "";

                //implementation description overrides interface description
                string description = implementation.GetCustomAttributeValue<string, OperationNotesAttribute>("Notes") ??
                    declaration.GetCustomAttributeValue<string, OperationNotesAttribute>("Notes") ??
                    implementation.GetCustomAttributeValue<string, DescriptionAttribute>("Description") ??
                    declaration.GetCustomAttributeValue<string, DescriptionAttribute>("Description") ??
                    "";

                string summary =
                    implementation.GetCustomAttributeValue<string, OperationSummaryAttribute>("Summary") ??
                    declaration.GetCustomAttributeValue<string, OperationSummaryAttribute>("Summary") ??
                    "";

                summary += _MarkTagged && methodTags.Any() ? "***" : string.Empty;

                var returnType =
                    implementation.GetCustomAttributeValue<Type, OverrideReturnTypeAttribute>("Type") ??
                    declaration.GetCustomAttributeValue<Type, OverrideReturnTypeAttribute>("Type") ??
                    declaration.ReturnType;

                var returnTypeString = HttpUtility.HtmlEncode(returnType.ToSwaggerType(typeStack));
                returnTypeString = returnType.GetCustomAttributeValue<string, DataContractAttribute>("Name") ?? returnTypeString;

                var operation = new APIOperation
                {
                    httpMethod = httpMethod,
                    nickname = declaration.Name + httpMethod,
                    type = returnTypeString,
                    summary = summary,
                    notes = description,
                    accepts = new List<string>(GetContentTypes<AcceptsAttribute>(implementation, declaration)),
                    produces = new List<string>(GetContentTypes<ProducesAttribute>(implementation, declaration))
                };
                if (declaration.ReturnType.IsArray)
                {
                    operation.itemsType = declaration.ReturnType.ToElementType(typeStack);

                }

                operation.errorResponses.AddRange(map.TargetMethods[index].GetResponseCodes());
                operation.errorResponses.AddRange(from r in map.InterfaceMethods[index].GetResponseCodes()
                                                  where !operation.errorResponses.Any(c => c.code.Equals(r.code))
                                                  select r);

                Uri uri = null;
                Uri.TryCreate(new Uri("http://base"), uriTemplate, out uri);

                var pathToReturn = uri.LocalPath;

                //try to map each implementation parameter to the uriTemplate.
                ParameterInfo[] parameters = declaration.GetParameters();
                foreach (ParameterInfo parameter in parameters)
                {
                    var typeValue = parameter.ParameterType.GetCustomAttributeValue<string, DataContractAttribute>("Name") ?? HttpUtility.HtmlEncode(parameter.ParameterType.ToSwaggerType(typeStack));

                    var parm = new APIParameter
                    {
                        name = parameter.Name,
                        allowMultiple = false,
                        required = true,
                        type = typeValue
                    };

                    //path parameters are simple
                    if (uri.LocalPath.Contains("{" + parameter.Name + "}"))
                    {
                        parm.paramType = "path";
                        parm.required = true;
                    }
                    //query parameters require checking and rewriting the name, as the query string name may not match the method signature name
                    else if (uri.Query.ToLower().Contains(HttpUtility.UrlEncode("{" + parameter.Name.ToLower() + "}")))
                    {
                        parm.paramType = "query";
                        parm.required = false;
                        string name = parameter.Name;
                        string paramName = (from p in HttpUtility.ParseQueryString(uri.Query).AllKeys
                                            where HttpUtility.ParseQueryString(uri.Query).Get(p).ToLower().Equals("{" + name.ToLower() + "}")
                                            select p).First();
                        parm.name = paramName;
                    }
                    //if we couldn't find it in the uri, it must be a body parameter
                    else
                    {
                        parm.paramType = "body";
                        parm.required = true;
                    }


                    var settings = implementation.GetParameters().First(p => p.Position.Equals(parameter.Position)).GetCustomAttribute<ParameterSettingsAttribute>() ??
                         parameter.GetCustomAttribute<ParameterSettingsAttribute>();
                    if (settings != null)
                    {
                        if (settings.Hidden)
                            continue;

                        parm.required = settings.IsRequired;
                        parm.description = settings.Description ?? parm.description;

                        var paramTypeToBeMapped = settings.UnderlyingType ?? parameter.ParameterType;
                        var paramTypeStringValue = HttpUtility.HtmlEncode(paramTypeToBeMapped.ToSwaggerType(typeStack, settings.TypeSizeNote));
                        var dataContractNameForParamType = paramTypeToBeMapped.GetCustomAttributeValue<string, DataContractAttribute>("Name");
                        if (!string.IsNullOrEmpty(dataContractNameForParamType))
                        {
                            paramTypeStringValue = string.IsNullOrEmpty(settings.TypeSizeNote)
                                ? dataContractNameForParamType
                                : string.Format("{0}({1})", dataContractNameForParamType, settings.TypeSizeNote);
                        }

                        parm.type = paramTypeStringValue;

                        if (parm.paramType == "query" && parm.required)
                        {
                            pathToReturn += pathToReturn.Contains("?") ? string.Format("&{0}={{{1}}}", parm.name, parm.name) : string.Format("?{0}={{{1}}}", parm.name, parm.name);
                        }
                    }

                    operation.parameters.Add(parm);
                }

                yield return new Tuple<string, APIOperation>(pathToReturn, operation);
            }
        }

        private IEnumerable<string> GetContentTypes<T>(MethodInfo implementation, MethodInfo declaration) where T : ContentTypeAttribute
        {
            if (implementation.GetCustomAttributes<T>().Any())
            {
                return implementation.GetCustomAttributes<T>().Select(a => a.ContentType);
            }
            else if (declaration.GetCustomAttributes<T>().Any())
            {
                return declaration.GetCustomAttributes<T>().Select(a => a.ContentType);
            }
            else
            {
                return new[] { "application/json", "application/xml" };
            }
        }

        private List<Core.Models.APIs.APIType> FindAPIModels(Stack<Type> typeStack)
        {
            var list = new List<APIType>();

            while (typeStack.Count > 0)
            {
                Type t = typeStack.Pop();
                if (t.GetCustomAttribute<HiddenAttribute>() != null) { continue; }
                if (t.GetCustomAttributes<TagAttribute>().Select(tn => tn.TagName).Any(_HiddenTags.Contains)) { continue; }

                list.Add(new APIType
                {
                    id = t.GetCustomAttributeValue<string, DataContractAttribute>("Name") ?? t.FullName,
                    properties = FindAPITypeProperties(t, typeStack)
                });
            }

            return list;
        }

        private List<APITypeProperty> FindAPITypeProperties(Type type, Stack<Type> typeStack)
        {
            var apiProperties = new List<APITypeProperty>();

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
                foreach (var pi in propertiesToWrite)
                {
                    if (pi.GetCustomAttribute<DataMemberAttribute>() == null
                        || pi.GetCustomAttribute<HiddenAttribute>() != null
                        || pi.GetCustomAttributes<TagAttribute>().Select(t => t.TagName).Any(_HiddenTags.Contains))
                        continue;

                    var dataMemberAttribute = pi.GetCustomAttribute<DataMemberAttribute>();
                    var propertyName = pi.Name;
                    if (dataMemberAttribute != null && !string.IsNullOrEmpty(dataMemberAttribute.Name))
                        propertyName = dataMemberAttribute.Name;

                    apiProperties.Add(FindProperty(pi, typeStack));
                }
            }

            return apiProperties;
        }

        private APITypeProperty FindProperty(PropertyInfo pi, Stack<Type> typeStack)
        {
            Type pType = pi.PropertyType;
            bool required = true;
            if (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(System.Nullable<>))
            {
                required = false;
                pType = pType.GetGenericArguments().First();
            }

            var memberProperties = pi.GetCustomAttribute<MemberPropertiesAttribute>();

            var typeValue = memberProperties != null ? pType.ToSwaggerType(typeStack, memberProperties.TypeSizeNote) : pType.ToSwaggerType(typeStack);

            //If the Name property in DataContract is defined for this property type, use that name instead.
            //Needed for cases where a DataContract uses another DataContract as a return type for a member. 
            var dcName = pType.GetCustomAttributeValue<string, DataContractAttribute>("Name");

            if (!string.IsNullOrEmpty(dcName))
            {
                typeValue = dcName;
                if (memberProperties != null && !string.IsNullOrEmpty(memberProperties.TypeSizeNote))
                    typeValue = string.Format("{0}({1})", dcName, memberProperties.TypeSizeNote);
            }

            string description = null;
            DescriptionAttribute descriptionAttr = pi.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr != null)
            {
                description = descriptionAttr.Description;
            }
            else
            {
                if (memberProperties != null)
                {
                    description = memberProperties.Description;
                }
            }


            var apiProperty = new APITypeProperty
            {
                type = typeValue,
                required = required,
                description = description
            };

            if (pType.ToSwaggerType(typeStack) == "array")
            {
                apiProperty.itemsType = pType.ToElementType(typeStack);
            }

            if (pType.IsEnum)
            {
                apiProperty.enumValues = pType.GetEnumNames().ToList();
            }

            return apiProperty;
        }
    }
}

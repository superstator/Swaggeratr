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
 * Discoverator.cs : Core Swaggerator service. Provides primary public methods for service discovery and description.
 */


using Swaggerator.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.IO;
using System.ServiceModel.Activation;
using Swaggerator.Core;
using Swaggerator.Core.Models.Services;
using Swaggerator.WCF.Reflector;

namespace Swaggerator.WCF
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Discoverator : IDiscoverator
    {
        /// <summary>
        /// Create a new instance of the core Discoverator service
        /// </summary>
        public Discoverator()
        {
            string sectionName = "swagger";
            var config = (Configuration.SwaggerSection)(System.Configuration.ConfigurationManager.GetSection(sectionName) ?? new Configuration.SwaggerSection());
            HiddenTags = config.Tags.OfType<Configuration.TagElement>().Where(t => t.Visibile.Equals(false)).Select(t => t.Name);
            _Serializer = new Serializer();
            _ServiceReflector = new ServiceReflector();
        }

        internal Discoverator(IEnumerable<string> hiddenTags, ISerializer serializer, IServiceReflector reflector)
        {
            HiddenTags = hiddenTags;
            _Serializer = serializer;
            _ServiceReflector = reflector;
        }

        internal readonly IEnumerable<string> HiddenTags;
        ISerializer _Serializer;
        IServiceReflector _ServiceReflector;

        public Stream GetServices()
        {
            var serviceList = _ServiceReflector.GetServices();

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(_Serializer.SerializeServiceList(serviceList)));

            return stream;
        }
                

        public Stream GetServiceDetails(string servicePath)
        {
            return GetServiceDetails(AppDomain.CurrentDomain, HttpContext.Current.Request.Url, servicePath);
        }

        public Stream GetServiceDetails(AppDomain domain, Uri baseUri, string servicePath)
        {
            Swaggerator.Core.Models.APIs.API serviceDetails = _ServiceReflector.GetAPIDetails(baseUri, servicePath);

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(_Serializer.SerializeAPIDetails(serviceDetails)));

            return stream;            
        }
    }
}

﻿/*
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
using Swaggerator.Core.Models.APIs;
using Swaggerator.Core.Models.Services;
using Swaggerator.Core.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Swaggerator.Core
{
    public class Serializer : Swaggerator.Core.ISerializer
    {
        public Serializer() : this(new ServiceListSerializer(), new APISerializer()) { }

        public Serializer(IServiceListSerializer serviceListSerializer, IAPISerializer apiSerializer)
        {
            _ServiceListSerializer = serviceListSerializer;
            _APISerializer = apiSerializer;
        }

        private IServiceListSerializer _ServiceListSerializer;
        private IAPISerializer _APISerializer;

        public string SerializeAPI(API api)
        {
            return _APISerializer.SerializeAPI(api);
        }

        public string SerializeServiceList(ServiceList serviceList)
        {
            return _ServiceListSerializer.SerializeServiceList(serviceList);
        }

        public string SerializeAPIDetails(API api)
        {
            return _APISerializer.SerializeAPI(api);
        }
    }
}
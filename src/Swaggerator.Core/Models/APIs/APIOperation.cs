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
 * Operation.cs : Operation model for serialization.
 */

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Swaggerator.Core.Models.APIs
{
    public class APIOperation
    {
        public APIOperation()
        {
            parameters = new List<APIParameter>();
            errorResponses = new List<APIResponseCode>();
            accepts = new List<string>();
            produces = new List<string>();
        }

        public string httpMethod { get; set; }
        public string nickname { get; set; }
        public string type { get; set; }
        public string itemsType { get; set; }
        public List<APIParameter> parameters { get; set; }
        public string summary { get; set; }
        public string notes { get; set; }
        public List<APIResponseCode> errorResponses { get; set; }
        public List<string> accepts { get; set; }
        public List<string> produces { get; set; }
    }
}
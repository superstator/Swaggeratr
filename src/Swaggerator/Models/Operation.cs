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

namespace Swaggerator.Models
{
    [DataContract]
    internal class Operation
    {
        public Operation()
        {
            parameters = new List<Parameter>();
            errorResponses = new List<ResponseCode>();
            accepts = new List<string>();
            produces = new List<string>();
        }

        [DataMember]
        public string httpMethod { get; set; }
        [DataMember]
        public string nickname { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public OperationItems items { get; set; }
        [DataMember]
        public List<Parameter> parameters { get; set; }
        [DataMember]
        public string summary { get; set; }
        [DataMember]
        public string notes { get; set; }
        [DataMember]
        public List<ResponseCode> errorResponses { get; set; }
        [DataMember]
        public List<string> accepts { get; set; }
        [DataMember]
        public List<string> produces { get; set; }
    }

    [DataContract]
    internal class OperationItems
    {
        [DataMember(Name = "$ref")]
        public string Ref { get; set; }
    }
}
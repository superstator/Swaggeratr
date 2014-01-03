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
 * ResponseCodeAttribute.cs : Attribute to document common method response codes.
 */

using System;

namespace Swaggerator.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ResponseCodeAttribute : Attribute
    {
        public ResponseCodeAttribute(int code, string reason = "")
        {
            Code = code;
            Description = reason;
        }

        public int Code { get; set; }

        public string Description { get; set; }
    }
}

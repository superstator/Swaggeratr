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
 * SwaggeratedAttribute.cs : Attribute to signal a given service should be included in generated Swagger docs.
 * 
 */

using System;

namespace Swaggerator.Attributes
{
    public class SwaggeratedAttribute : Attribute
    {
        /// <summary>
        /// Make this service "discoverable" by Swagger. Optional Description overrides DataAnnotations
        /// </summary>
        public SwaggeratedAttribute(string localPath = "", string description = null)
        {
            LocalPath = localPath;
            Description = description;
        }

        /// <summary>
        /// Relative path to this service.
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// Short description. Overrides DataAnnotaions.Description.
        /// </summary>
        public string Description { get; set; }
    }
}

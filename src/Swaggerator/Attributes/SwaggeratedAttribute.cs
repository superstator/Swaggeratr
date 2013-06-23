using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Configuration
{
    public class SwaggerSection : ConfigurationSection
    {
        [ConfigurationProperty("tag")]
        public TagElement Tag
        {
            get { return (TagElement)this["tag"]; }
            set { this["tag"] = value; }
        }

        [ConfigurationProperty("serviceName", IsRequired = true, DefaultValue = "*")]
        public string ServiceName
        {
            get { return (string)this["serviceName"]; }
            set { this["serviceName"] = value; }
        }
    }
}

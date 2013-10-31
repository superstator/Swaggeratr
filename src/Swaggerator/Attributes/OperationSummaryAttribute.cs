using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swaggerator.Attributes
{
    public class OperationSummaryAttribute : Attribute
    {
        public OperationSummaryAttribute(string summary)
        {
            Summary = summary;
        }

        public string Summary { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swaggerator.Core
{
    public interface IAPIResourceListSerializer
    {
        string SerializeAPIResources(List<Models.APIs.APIResource> list);
    }
}

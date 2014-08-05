using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swaggerator.Core
{
    public interface IAPITypeListSerializer
    {
        string SerializeAPITypes(List<Models.APIs.APIType> list);
    }
}

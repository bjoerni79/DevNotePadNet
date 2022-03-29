using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Xml
{
    public interface IXmlXPathQuery
    {
        XPathQueryResponse Query (XPathQueryRequest request);
    }
}

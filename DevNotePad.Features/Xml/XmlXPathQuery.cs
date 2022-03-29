using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Xml
{
    internal class XmlXPathQuery : IXmlXPathQuery
    {
        internal XmlXPathQuery()
        {

        }

        public XPathQueryResponse Query(XPathQueryRequest request)
        {
            // see XPathDocument class : https://docs.microsoft.com/en-us/dotnet/api/system.xml.xpath.xpathdocument?view=net-6.0
            // and also: https://docs.microsoft.com/en-us/dotnet/api/system.xml.xpath.xpathnavigator?view=net-6.0

            throw new NotImplementedException();
        }
    }
}

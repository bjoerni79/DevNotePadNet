using DevNotePad.Features.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Xml
{
    internal class XmlComponent : IXmlComponent
    {
        internal XmlComponent()
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-6.0#xmlreader_nodes
        }

        public string Formatter(string xmlText)
        {
            throw new NotImplementedException();
        }

        public string ParseToString(string xmlText)
        {
            throw new NotImplementedException();
        }

        public ItemNode ParseToTree(string xmlText)
        {
            throw new NotImplementedException();
        }
    }
}

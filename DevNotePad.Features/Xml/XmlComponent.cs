using DevNotePad.Features.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
            ItemNode rootNode = new ItemNode();
            var settings = GetReaderSettings();

            using (var textReader = new StringReader(xmlText))
            using (var xmlreader = XmlReader.Create(textReader,settings))
            {
                while (xmlreader.Read())
                {
                    // TODO: Group the XML Node Types
                    // https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlnodetype?f1url=%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(System.Xml.XmlNodeType);k(DevLang-csharp)%26rd%3Dtrue&view=net-6.0
                }
            }

            return rootNode;
        }

        private XmlReaderSettings GetReaderSettings()
        {
            var settings = new XmlReaderSettings();

            //TODO...

            return settings;
        }
    }
}

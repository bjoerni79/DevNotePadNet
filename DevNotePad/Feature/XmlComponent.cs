using DevNotePad.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DevNotePad.Feature
{
    internal class XmlComponent : IXmlComponent
    {
        internal XmlComponent()
        {

        }

        public string Formatter(string xmlText)
        {
            string result;
            var document = Read(xmlText);
            var writerSettings = new XmlWriterSettings() { Indent = true, NewLineHandling = NewLineHandling.Entitize };

            try
            {
                using var stream = new MemoryStream();
                using (var writer = XmlWriter.Create(stream, writerSettings)) 
                {
                    //TODO:  Check this code. ResultArray is empty.
                    document.WriteTo(writer);

                    var resultArray = stream.ToArray();
                    result = Encoding.UTF8.GetString(resultArray);
                }
            }
            catch (Exception e)
            {
                throw new FeatureException("Cannot format XML", e);
            }

            return result;
        }

        public string ParseToString(string xmlText)
        {
            throw new NotImplementedException();
        }

        public ItemNode ParseToTree(string xmlText)
        {
            ItemNode rootNode;
            var document = Read(xmlText);

            try
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlnode?view=net-6.0

                rootNode = ParseDom(document);
            }
            catch (Exception e)
            {
                throw new FeatureException("Cannot parse XML format", e);
            }

            return rootNode;
        }

        ItemNode ParseDom(XmlNode node)
        {
            bool hasChilds = node.HasChildNodes;
            var namespaceUri = node.NamespaceURI;
            var name = node.Name;
            var attributes = node.Attributes;

            return new ItemNode() { Name = name, Description = "TODO" };
        }

        private XmlDocument Read (string xmlText)
        {
            XmlDocument document;

            try
            {
                document = new XmlDocument();
                document.LoadXml(xmlText);
            }
            catch (Exception e)
            {
                throw new FeatureException("Could not load XML document",e);
            }

            return document;
        }
    }
}

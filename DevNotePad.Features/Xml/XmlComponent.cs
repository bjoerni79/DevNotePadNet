using DevNotePad.Features.Shared;
using System;
using System.Collections.Generic;
using System.IO;
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
                    document.WriteContentTo(writer);
                    writer.Flush();
                }

                var resultArray = stream.ToArray();
                result = Encoding.UTF8.GetString(resultArray);
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
            ItemNode itemRootNode;
            var document = Read(xmlText);

            try
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlnode?view=net-6.0

                itemRootNode = ParseDom(document);
            }
            catch (Exception e)
            {
                throw new FeatureException("Cannot parse XML format", e);
            }

            return itemRootNode;
        }

        ItemNode ParseDom(XmlNode node)
        {
            var itemNode = new ItemNode();
            var nodeType = node.NodeType;

            // Root Node of the DOM
            if (nodeType == XmlNodeType.Document)
            {
                itemNode.Name = "Root";
                itemNode.Description = String.Empty;
                itemNode.Style = ItemNodeStyle.Group;
            }
            else
            {
                if (nodeType == XmlNodeType.XmlDeclaration)
                {
                    itemNode.Name = "Declaration";
                }

                // Element Node of the DOM
                if (nodeType == XmlNodeType.Element)
                {
                    itemNode.Name = node.Name;
                }

                // Text
                if (nodeType == XmlNodeType.Text)
                {
                    itemNode.Name = "Text";
                    itemNode.Description = node.InnerText;
                }
            }


            bool hasChilds = node.HasChildNodes;
            if (hasChilds)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    var childItem = ParseDom(child);
                    itemNode.Childs.Add(childItem);
                }
            }

            var namespaceUri = node.NamespaceURI;
            var name = node.Name;
            var attributes = node.Attributes;

            return itemNode;
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

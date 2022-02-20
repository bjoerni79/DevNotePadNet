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
    internal class XmlComponentOld : IXmlComponent
    {
        internal XmlComponentOld()
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
            var rootNodeList = ParseToTree(xmlText);
            var converter = new ItemNodeConverter();
            return converter.ToTreeAsString(rootNodeList.First()); ;
        }

        public IEnumerable<ItemNode> ParseToTree(string xmlText)
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

            return new List<ItemNode>() { itemRootNode };
        }

        ItemNode ParseDom(XmlNode node)
        {
            ItemNode itemNode;
            var nodeType = node.NodeType;

            var hasChild = node.HasChildNodes;
            var isDocument = nodeType == XmlNodeType.Document;
            var isElement = nodeType == XmlNodeType.Element; ;
            var isMeta = nodeType == XmlNodeType.XmlDeclaration || nodeType == XmlNodeType.Comment;
            var isText = nodeType == XmlNodeType.Text;

            if (isMeta)
                itemNode = HandleMetaDeclaration(node);
            else
            {
                itemNode = new ItemNode();

                if (isElement)
                {
                    var attributes = node.Attributes;
                    if (attributes != null && attributes.Count > 0)
                    {
                         AddAttributes(node, itemNode);
                    }

                    itemNode.Name = node.Name;
                    itemNode.Style = ItemNodeStyle.Element;
                }

                if (isDocument)
                {
                    itemNode.Name = "Root";
                    itemNode.Style = ItemNodeStyle.Title; ;
                }

                // Iterate over childs
                if (hasChild)
                {
                    foreach (XmlNode childXmlNode in node.ChildNodes)
                    {
                        var child = ParseDom(childXmlNode);
                        itemNode.Childs.Add(child);
                    }
                }

                if (isText)
                {
                    itemNode.Name = node.InnerText;
                    itemNode.Style = ItemNodeStyle.Value;
                }
            }

            return itemNode;
        }

        private void AddAttributes(XmlNode xmlNode, ItemNode itemNode)
        {
            if (xmlNode.Attributes != null && xmlNode.Attributes.Count > 0)
            {
                foreach (XmlAttribute curattrbute in xmlNode.Attributes)
                {
                    var attributeNode = new ItemNode();
                    var name = curattrbute.Name;
                    var value = curattrbute.Value;

                    attributeNode.Name = name;
                    attributeNode.Description = value;
                    attributeNode.Style = ItemNodeStyle.Attribute;

                    itemNode.Childs.Add(attributeNode);
                }
            }
        }

        private ItemNode HandleMetaDeclaration(XmlNode node)
        {
            var itemNode = new ItemNode();
            itemNode.Style = ItemNodeStyle.Default;

            switch (node.NodeType)
            {
                case XmlNodeType.XmlDeclaration:
                    itemNode.Name = "XML Declare";
                    break;
                case XmlNodeType.Comment:
                    itemNode.Name = "Comment";
                    itemNode.Description = node.InnerText;
                    break;
                default:
                    itemNode.Name = node.Name;
                    break;
            }


            return itemNode;
        }

        private XmlDocument Read (string xmlText)
        {
            XmlDocument document;

            try
            {
                // StringReader: https://docs.microsoft.com/en-us/dotnet/api/system.io.stringreader?view=net-6.0
                // https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmldocument.load?view=net-6.0#system-xml-xmldocument-load(system-xml-xmlreader)
                // TODO: Try using the Load() + XmlReader 

                var readerSettings = new XmlReaderSettings()
                {
                    IgnoreWhitespace = true,
                    IgnoreComments = true,
                    IgnoreProcessingInstructions = true,
                    ConformanceLevel = ConformanceLevel.Auto,
                    DtdProcessing = DtdProcessing.Parse,
                };

                document = new XmlDocument();

                var reader = XmlReader.Create(new StringReader(xmlText),readerSettings);
                document.Load(reader);
                //document.LoadXml(xmlText);
            }
            catch (XmlException xmlException)
            {
                var featureException = new FeatureException("Could not load XML", xmlException);

                var details = String.Format("Line : {0}\nPosition : {1}\nMessage = {2}", xmlException.LineNumber, xmlException.LinePosition, xmlException.Message);
                featureException.Details = details;

                throw featureException;
            }
            catch (Exception e)
            {
                throw new FeatureException("Generic Error while reading XML",e);
            }

            return document;
        }

        public Task<IEnumerable<ItemNode>> ParseToTreeAsync(string xmlText)
        {
            throw new NotImplementedException();
        }
    }
}

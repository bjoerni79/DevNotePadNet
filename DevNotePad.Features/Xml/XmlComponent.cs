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
            // Open the XML Reader
            var readerSettings = GetReaderSettings();
            using (var textReader = new StringReader(xmlText))
            using (var xmlreader = XmlReader.Create(textReader, readerSettings))
            {
                while (xmlreader.Read())
                {
                    // TODO: Group the XML Node Types
                    // https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlnodetype?f1url=%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(System.Xml.XmlNodeType);k(DevLang-csharp)%26rd%3Dtrue&view=net-6.0

                    var nodeType = xmlreader.NodeType;
                    
                    switch (nodeType)
                    {
                        case XmlNodeType.Element:
                            break;
                        case XmlNodeType.Attribute:
                            break;
                        case XmlNodeType.Text:
                            break;
                        case XmlNodeType.CDATA:
                            break;
                        case XmlNodeType.EntityReference:
                            break;
                        case XmlNodeType.Entity:
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            break;
                        case XmlNodeType.Comment:
                            break;
                        case XmlNodeType.Document:
                            break;
                        case XmlNodeType.DocumentFragment:
                            break;
                        case XmlNodeType.DocumentType:
                            break;
                        case XmlNodeType.Notation:
                            break;
                        case XmlNodeType.Whitespace:
                            break;
                        case XmlNodeType.SignificantWhitespace:
                            break;
                        case XmlNodeType.EndElement:
                            break;
                        case XmlNodeType.EndEntity:
                            break;
                        case XmlNodeType.XmlDeclaration:
                            xmlreader.Skip();
                            break;
                        default:
                            throw new FeatureException("Unknown XML Node detected" + nodeType);
                    }
                }
            }


            return String.Empty;
        }

        public string ParseToString(string xmlText)
        {
            throw new NotImplementedException();
        }

        /*
         *  TODO: Group the XML Node Types
         *  https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlnodetype?f1url=%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(System.Xml.XmlNodeType);k(DevLang-csharp)%26rd%3Dtrue&view=net-6.0
         *  None
         *  X Element
         *  X Attribute
         *  X Text
         *  X CDATA
         *  X Entity Reference
         *  X Entity
         *  X Processing Instruction
         *  X Comment
         *  X Document
         *  X DocumentFragment
         *  X DocumentType
         *  X Notation
         *  X Whitespace
         *  X SignificantWhitespace
         *  X EndElement
         *  X EndEntity
         *  X XmlDeclaration
         * 
         */

        public IEnumerable<ItemNode> ParseToTree(string xmlText)
        {
            var parseTask = Task.Run(()=>ParseToTreeAsync(xmlText));
            parseTask.Wait();

            return parseTask.Result;
        }

        /*
         * 
         *   - XmlDeclaration
         *   - Whitespace
         *   - Element Menu
         *   - Whitespace
         *   - Element Popup
         *   - Whitespace
         *   - Element MenuItem
         *   - Whitespace
         *   - Element MenuItem
         *   - Whitespace
         *   - Element MenuItem
         *   - Whitespace
         *   - EndElement
         *   - Whitespace
         *   - EndElement 
         * 
         */

        private async Task<IEnumerable<ItemNode>> ParseToTreeAsync(string xmlText)
        {
            var settings = GetReaderSettings();

            var itemNodes = new List<ItemNode>();
            var nodeTypes = new List<XmlNodeType>();

            using (var textReader = new StringReader(xmlText))
            using (var xmlreader = XmlReader.Create(textReader, settings))
            {
                while (await xmlreader.ReadAsync())
                {
                    // Query the state 
                    var style = xmlreader.NodeType;
                    var isElement = (style == XmlNodeType.Element) || (style == XmlNodeType.EndElement);
                    var isAttribute = (style == XmlNodeType.Attribute);
                    var isText = (style == XmlNodeType.Text);
                    var isDocument = (style == XmlNodeType.Document) || (style == XmlNodeType.DocumentFragment) || (style == XmlNodeType.DocumentType);
                    var isEntity = (style == XmlNodeType.Entity) || (style == XmlNodeType.EndEntity) || (style == XmlNodeType.EntityReference);
                    var isXmlDeclaration = style == XmlNodeType.XmlDeclaration;
                    var isComment = style == XmlNodeType.Comment;
                    var isOther = (style == XmlNodeType.CDATA) || (style == XmlNodeType.ProcessingInstruction) || (style == XmlNodeType.Notation) || (style == XmlNodeType.Whitespace) || (style == XmlNodeType.SignificantWhitespace);

                    if (isXmlDeclaration)
                    {
                    }

                    if (isDocument)
                    {
                    }

                    if (isElement)
                    {

                    }

                    if (isAttribute)
                    {
                        //xmlreader.N
                    }

                    nodeTypes.Add(style);
                }
            }

            Debug(nodeTypes);

            return itemNodes;
        }

        private XmlWriterSettings GetWriterSettings()
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = true;
            
            //TODO

            return settings;
        }

        private XmlReaderSettings GetReaderSettings()
        {
            var settings = new XmlReaderSettings();
            settings.Async = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.DtdProcessing = DtdProcessing.Ignore;
            
            //TODO...

            return settings;
        }

        private void Debug(IEnumerable<XmlNodeType> detectedTypes)
        {
            var file = @"D:\temp\xmlcomponent_debug.txt";

            try
            {
                using (var streamWriter = File.CreateText(file))
                {
                    foreach (var detectedType in detectedTypes)
                    {
                        streamWriter.WriteLine("- " + detectedType.ToString());
                    }
                }
            }
            catch (IOException ioEx)
            {
                // None... eat it and enjoy. Some red whine?
            }
        }
    }
}

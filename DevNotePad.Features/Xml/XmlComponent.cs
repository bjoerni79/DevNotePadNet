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
        private List<ItemNode>? itemNodes;
        private List<ItemNode>? parents;

        internal XmlComponent()
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-6.0#xmlreader_nodes
        }

        public async Task<string> FormatterAsync(string xmlText)
        {
            // Open the XML Reader
            var sb = new StringBuilder();

            var readerSettings = GetReaderSettings();
            var writerSettings = GetWriterSettings();

            using (var textReader = new StringReader(xmlText))
            using (var textWriter = new StringWriter(sb))
            using (var xmlWriter = XmlWriter.Create(textWriter, writerSettings))
            using (var xmlreader = XmlReader.Create(textReader, readerSettings))
            {
                while (await xmlreader.ReadAsync())
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
                            //xmlWriter.
                            break;
                        default:
                            throw new FeatureException("Unknown XML Node detected" + nodeType);
                    }
                }
            }

            return sb.ToString();
        }

        public string Formatter(string xmlText)
        {
            var formatterTask = Task.Run(() => FormatterAsync(xmlText));
            formatterTask.Wait();

            return formatterTask.Result;
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

        public async Task<IEnumerable<ItemNode>> ParseToTreeAsync(string xmlText)
        {
            var settings = GetReaderSettings();

            itemNodes = new List<ItemNode>();
            //var nodeTypes = new List<XmlNodeType>();
            parents = new List<ItemNode>();

            using (var textReader = new StringReader(xmlText))
            using (var xmlreader = XmlReader.Create(textReader, settings))
            {
                while (await xmlreader.ReadAsync())
                {
                    // Query the state 
                    var currentType = xmlreader.NodeType;
                    var isElement = (currentType == XmlNodeType.Element) || (currentType == XmlNodeType.EndElement);
                    var isDocument = (currentType == XmlNodeType.Document) || (currentType == XmlNodeType.DocumentFragment) || (currentType == XmlNodeType.DocumentType);
                    var isEntity = (currentType == XmlNodeType.Entity) || (currentType == XmlNodeType.EndEntity) || (currentType == XmlNodeType.EntityReference);
                    var isXmlDeclaration = currentType == XmlNodeType.XmlDeclaration;
                    var isComment = currentType == XmlNodeType.Comment;
                    var isText = currentType == XmlNodeType.Text;
                    var isCDATA = currentType == XmlNodeType.CDATA;
 
                    // Ignore some elements...
                    if (isEntity || isDocument || isCDATA)
                    {
                        await xmlreader.SkipAsync();
                    }

                    // isEmpty = true :   <tag />
                    // isEmpty = false :  <tag> ... </tag>
                    var isEmpty = xmlreader.IsEmptyElement;
                    var hasValue = xmlreader.HasValue;

                    if (isXmlDeclaration)
                    {
                        var xmlDeclareNode = new ItemNode();
                        xmlDeclareNode.Style = ItemNodeStyle.Meta;
                        xmlDeclareNode.Name = "XML Declaration";

                        itemNodes.Add(xmlDeclareNode);
                    }

                    if (isText)
                    {
                        
                        if (hasValue)
                        {
                            var textValue = await xmlreader.GetValueAsync();
                            var textNode = new ItemNode() { Style = ItemNodeStyle.Value, Name = "Content", Description=textValue };

                            AddItemNode(textNode, isEmpty);
                        }
                    }

                    //if (isComment)
                    //{
                    //    if (hasValue)
                    //    {
                    //        var commentValue = await xmlreader.GetValueAsync();

                    //    }
                    //}

                    if (isElement)
                    {
                        // Check the element state
                        if (currentType == XmlNodeType.Element)
                        {
                            var elementNode = new ItemNode();
                            elementNode.Name = xmlreader.Name;
                            elementNode.Style = ItemNodeStyle.Element;

                            // Read the attributes, if available
                            var hasAttributes = xmlreader.AttributeCount > 0; 
                            if (hasAttributes)
                            {
                                for (int curAttribute = 0; curAttribute < xmlreader.AttributeCount; curAttribute++)
                                {
                                    xmlreader.MoveToAttribute(curAttribute);

                                    var attributeNote = new ItemNode();
                                    attributeNote.Name = xmlreader.Name;
                                    attributeNote.Description = xmlreader.Value;
                                    attributeNote.Style = ItemNodeStyle.Attribute;

                                    elementNode.Childs.Add(attributeNote);
                                }
                            }

                            AddItemNode(elementNode,isEmpty);
                        }
                        else if (currentType == XmlNodeType.EndElement)
                        {
                            // Remove the parent from the list
                            parents = parents.SkipLast(1).ToList();
                        }
                        else
                        {
                            throw new FeatureException("Invalid End Element detected. It is neither Element nor EndElement.");
                        }
                    }

                    //nodeTypes.Add(currentType);
                }
            }

            //Debug(nodeTypes);

            return itemNodes;
        }

        private void AddItemNode(ItemNode elementNode, bool isEmpty)
        {
            // Build a hierarchy
            if (parents.Any())
            {
                // Get the last parent and add the element
                var parent = parents.Last();
                parent.Childs.Add(elementNode);

                if (!isEmpty)
                {
                    parents.Add(elementNode);
                }
            }
            else
            {
                // Go to the root Root Level
                // 
                itemNodes.Add(elementNode);

                // Add this one as the new parent
                if (!isEmpty)
                {
                    parents.Add(elementNode);
                }

            }
        }

        private XmlWriterSettings GetWriterSettings()
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.Async = true;
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



        //private void Debug(IEnumerable<XmlNodeType> detectedTypes)
        //{
        //    var file = @"D:\temp\xmlcomponent_debug.txt";

        //    try
        //    {
        //        using (var streamWriter = File.CreateText(file))
        //        {
        //            foreach (var detectedType in detectedTypes)
        //            {
        //                streamWriter.WriteLine("- " + detectedType.ToString());
        //            }
        //        }
        //    }
        //    catch (IOException ioEx)
        //    {
        //        // None... eat it and enjoy. Some red whine?
        //    }
        //}

    }
}

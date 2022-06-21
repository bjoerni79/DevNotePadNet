using DevNotePad.Features.Shared;
using System.Text;
using System.Xml;

namespace DevNotePad.Features.Xml
{
    internal class XmlComponent : IXmlComponent
    {
        private List<ItemNode>? itemNodes;
        private List<ItemNode>? parents;

        // References:
        // XmlReader: https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-6.0#xmlreader_nodes

        internal XmlComponent()
        {
            // Empty
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
                    // Copy the reader and format it based on the ruling in the XmlWriter.
                    await xmlWriter.WriteNodeAsync(xmlreader, true);
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



        public IEnumerable<ItemNode> ParseToTree(string xmlText)
        {
            var parseTask = Task.Run(() => ParseToTreeAsync(xmlText));
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
                            var textNode = new ItemNode() { Style = ItemNodeStyle.Value, Name = "Content", Description = textValue };

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

                            AddItemNode(elementNode, isEmpty);
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
            settings.NewLineHandling = NewLineHandling.Entitize;
            //settings.NewLineOnAttributes = true;
            //settings.OmitXmlDeclaration = true;
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


    }
}

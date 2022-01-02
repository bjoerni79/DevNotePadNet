using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using DevNotePad.Features.Shared;

namespace DevNotePad.Features.Json
{
    internal class JsonComponent : IJsonComponent
    {
        internal JsonComponent()
        {

        }

        public string Formatter(string jsonText)
        {
            // Read the JSON text 
            JsonDocument document = Read(jsonText);
            string result;

            // Format it
            try
            {
                var writerOptions = new JsonWriterOptions() { Indented = true, SkipValidation = true };

                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream, writerOptions))
                {
                    document.WriteTo(writer);
                    writer.Flush();
                }

                var bytes = stream.ToArray();
                result = Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                throw new FeatureException("Cannot format JSON coding", ex);
            }

            return result;
        }

        public string ParseToString(string jsonText)
        {
            // Read the JSON text
            JsonDocument document = Read(jsonText);

            var builder = new StringBuilder();
            var root = document.RootElement;

            DomParserOld(root, builder);
            return builder.ToString();
        }

        public ItemNode ParseToTree(string jsonText)
        {
            // Read the JSON text
            JsonDocument document = Read(jsonText);
            var root = document.RootElement;
            var rootNode = new ItemNode();

            DomParser(root, rootNode);
            return rootNode;
        }

        private void DomParser (JsonElement element, ItemNode node)
        {
            var kind = element.ValueKind;
            if (kind == JsonValueKind.Array)
            {
                node.Name = "Array";
                node.Description = String.Empty;
                node.Style = ItemNodeStyle.Group;

                // Iterate over the childs
                foreach (var childElement in element.EnumerateArray())
                {
                    var childNode = new ItemNode();
                    DomParser(childElement, childNode);

                    node.Childs.Add(childNode);
                }
            }
            else
            {
                // Object
                node.Name = "Object";
                node.Description = String.Empty;

                foreach (var parameter in element.EnumerateObject())
                {
                    var childNode = new ItemNode();
                    childNode.Name = parameter.Name;
                    node.Childs.Add(childNode);

                    // Next steps are depending on the parameter value
                    var parameterValue = parameter.Value;

                    // Next iteration ..
                    if (parameterValue.ValueKind == JsonValueKind.Array)
                    {
                        // Value is a array
                        childNode.Description = String.Empty;
                        childNode.Style = ItemNodeStyle.Group;

                        foreach (var parameterChildNode in parameterValue.EnumerateArray())
                        {
                            var parameterChild = new ItemNode();
                            DomParser(parameterChildNode, parameterChild);

                            childNode.Childs.Add(parameterChild);
                        }
                    }
                    else if (parameterValue.ValueKind == JsonValueKind.Object)
                    {
                        // Value is another object
                        var parameterChild = new ItemNode();
                        DomParser(parameterValue, parameterChild);

                        childNode.Childs.Add(parameterChild);
                    }
                    else
                    {
                        // String, Int, ....
                        var stringBuilder = new StringBuilder();
                        RenderValue(parameterValue, stringBuilder);

                        childNode.Description = stringBuilder.ToString();
                    }
                }
            }
        }

        private void DomParserOld(JsonElement element, StringBuilder builder)
        {
            var kind = element.ValueKind;
            if (kind == JsonValueKind.Array)
            {
                //
                // Recursive scan of the array
                //

                var arrayLength = element.GetArrayLength();
                if (arrayLength > 0)
                {
                    foreach (var childElement in element.EnumerateArray())
                    {
                        builder.AppendLine("## Parse new Object");
                        DomParserOld(childElement, builder);
                    }
                }
                else
                {
                    builder.AppendLine("## Empty Array declaration detected.");
                }
            }
            else
            {
                //
                //  Parse the object 
                //

                foreach (var parameter in element.EnumerateObject())
                {
                    builder.AppendFormat("Name : {0}\n", parameter.Name);
                    var parameterValue = parameter.Value;

                    if (parameterValue.ValueKind == JsonValueKind.Array)
                    {
                        // Render as Array
                        var arrayLength = parameterValue.GetArrayLength();
                        if (arrayLength > 0)
                        {
                            foreach (var childElement in parameterValue.EnumerateArray())
                            {
                                builder.AppendLine("## Parse new Object");
                                DomParserOld(childElement, builder);
                            }

                        }
                        else
                        {
                            builder.AppendLine("## Empty Array declaration detected.");
                        }
                    }
                    else
                    {
                        // Render as Object
                        RenderValue(parameterValue, builder);
                    }
                }
            }
        }

        private void RenderValue(JsonElement element, StringBuilder builder)
        {
            var kind = element.ValueKind;
            if (kind == JsonValueKind.Object || kind == JsonValueKind.Array)
            {
                builder.AppendFormat("## Object/ Array detected\n");
            }

            if (kind == JsonValueKind.Null)
            {
                builder.AppendFormat("## Null detected\n");
            }

            if (kind == JsonValueKind.Number)
            {
                builder.AppendFormat("Value = {0}\n", element.GetInt32());
            }

            if (kind == JsonValueKind.String)
            {
                builder.AppendFormat("Value = {0}\n", element.GetString());
            }

            if (kind == JsonValueKind.True)
            {
                builder.AppendLine("Value is true");
            }

            if (kind == JsonValueKind.False)
            {
                builder.AppendLine("Value is false");
            }
        }

        private JsonDocument Read(string jsonCoding)
        {
            JsonDocument document;

            try
            {
                var options = new JsonDocumentOptions() { AllowTrailingCommas = true };
                document = JsonDocument.Parse(jsonCoding, options);
            }
            catch (Exception ex)
            {
                throw new FeatureException("Cannot read JSON coding", ex);
            }

            return document;
        }


    }
}

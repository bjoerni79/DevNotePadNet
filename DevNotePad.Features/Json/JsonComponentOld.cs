﻿using DevNotePad.Features.Shared;
using System.Text;
using System.Text.Json;

namespace DevNotePad.Features.Json
{
    internal class JsonComponentOld : IJsonComponent
    {
        internal JsonComponentOld()
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
            var rootNode = ParseToTree(jsonText);
            var converter = new ItemNodeConverter();
            return converter.ToTreeAsString(rootNode);
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

        private void DomParser(JsonElement element, ItemNode node)
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
            else if (kind == JsonValueKind.Object)
            {
                // Object
                node.Name = "Object";
                node.Description = String.Empty;
                node.Style = ItemNodeStyle.Element;

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

                        childNode.Style = ItemNodeStyle.Value;
                        childNode.Description = stringBuilder.ToString();
                    }
                }
            }
            else
            {
                // Value
                var sb = new StringBuilder();
                RenderValue(element, sb);

                node.Style = ItemNodeStyle.Value;
                node.Name = sb.ToString();
            }
        }

        private void RenderValue(JsonElement element, StringBuilder builder)
        {
            var kind = element.ValueKind;
            if (kind == JsonValueKind.Object || kind == JsonValueKind.Array)
            {
                builder.AppendFormat("Object/ Array\n");
            }

            if (kind == JsonValueKind.Null)
            {
                builder.AppendFormat("Null\n");
            }

            if (kind == JsonValueKind.Number)
            {
                builder.AppendFormat("Value = {0}\n", element.GetInt64());
            }

            if (kind == JsonValueKind.String)
            {
                builder.AppendFormat("Value = {0}\n", element.GetString());
            }

            if (kind == JsonValueKind.True)
            {
                builder.AppendLine("True");
            }

            if (kind == JsonValueKind.False)
            {
                builder.AppendLine("False");
            }
        }

        private JsonDocument Read(string jsonCoding)
        {
            JsonDocument document;

            try
            {
                // JSON files with comments cannot be loaded. The only option is to ignore them.
                // The Format option deletes them from the formatted version later on.
                // https://docs.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions.commenthandling?view=net-6.0#system-text-json-jsondocumentoptions-commenthandling

                var options = new JsonDocumentOptions() { AllowTrailingCommas = true };
                options.CommentHandling = JsonCommentHandling.Skip;

                document = JsonDocument.Parse(jsonCoding, options);
            }
            catch (JsonException jsonException)
            {
                var featureException = new FeatureException("Cannot parse JSON coding", jsonException);

                var detailsBuilder = new StringBuilder();
                if (jsonException.Message != null)
                {
                    detailsBuilder.AppendLine(jsonException.Message);
                }

                if (jsonException.LineNumber.HasValue)
                {
                    detailsBuilder.AppendFormat("Line : {0}\n", jsonException.LineNumber.Value + 1);
                }

                if (jsonException.BytePositionInLine.HasValue)
                {
                    detailsBuilder.AppendFormat("Position in Line: {0}", jsonException.BytePositionInLine.Value + 1);
                }

                featureException.Details = detailsBuilder.ToString();
                throw (featureException);
            }
            catch (Exception ex)
            {
                throw new FeatureException("Generic Issue during pasing JSON content", ex);
            }

            return document;
        }


    }
}

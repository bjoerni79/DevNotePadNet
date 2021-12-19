using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace DevNotePad.Feature
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

        public string Parse(string jsonText)
        {
            // Read the JSON text
            JsonDocument document = Read(jsonText);

            var builder = new StringBuilder();
            var root = document.RootElement;

            DomParser(root, builder);
            return builder.ToString();
        }

        private void DomParser(JsonElement element, StringBuilder builder)
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
                        builder.AppendLine("Parse new Object");
                        DomParser(childElement, builder);
                    }
                }
                else
                {
                    builder.AppendLine("Empty Array declaration detected.");
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
                                builder.AppendLine("Parse new Object");
                                DomParser(childElement, builder);
                            }

                        }
                        else
                        {
                            builder.AppendLine("Empty Array declaration detected.");
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
                builder.AppendFormat("Object/ Array detected\n");
            }

            if (kind == JsonValueKind.Null)
            {
                builder.AppendFormat("Null detected\n");
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

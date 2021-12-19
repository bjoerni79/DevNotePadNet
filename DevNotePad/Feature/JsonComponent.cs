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

        public string ParserTest(string jsonText)
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
                builder.AppendFormat("Array with {0} items\n", element.GetArrayLength());

                //foreach (var childObject in element.EnumerateArray())
                //{
                //    builder.AppendFormat("- Array: {0}\n",childObject.);
                //    DomParser(childObject, builder);
                //}
            }
            else
            {
                builder.AppendLine("Object");

                //foreach(var property in element.EnumerateObject())
                //{
                //    builder.AppendFormat("Value : {0} : {1}\n", property.Name, property.Value);
                //}
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

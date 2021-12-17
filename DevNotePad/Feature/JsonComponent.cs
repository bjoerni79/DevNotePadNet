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

            JsonDocument document;
            string result;

            try
            {
                var options = new JsonDocumentOptions() { AllowTrailingCommas = true };
                document = JsonDocument.Parse(jsonText, options);
            }
            catch (Exception ex)
            {
                throw new FeatureException("Cannot read JSON coding", ex);
            }

            // Format it

            try
            {
                var writerOptions = new JsonWriterOptions() { Indented = true, SkipValidation = true };
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream, writerOptions))
                    {
                        document.WriteTo(writer);
                    }

                    var bytes = stream.ToArray();
                    result = Encoding.UTF8.GetString(bytes);
                }
            }
            catch (Exception ex)
            {
                throw new FeatureException("Cannot format JSON coding", ex);
            }

            return result;
        }
    }
}

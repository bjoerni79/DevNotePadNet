using System.Xml;
using System.Xml.Schema;

namespace DevNotePad.Features.Xml
{
    internal class XmlSchemaValidator : IXmlSchemaValidator
    {
        private XmlReaderSettings readerSettings;
        private SchemaCompareResult? result;

        private List<SchemaValidationItem>? validationItems;

        internal XmlSchemaValidator()
        {
            readerSettings = new XmlReaderSettings()
            {
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings,
                Async = true,
            };

            readerSettings.ValidationEventHandler += ReaderSettings_ValidationEventHandler;
        }

        public async Task<SchemaCompareResult> CompareAsync(SchemaCompareRequest request)
        {
            validationItems = new List<SchemaValidationItem>();

            try
            {
                var xmlSchema = XmlSchema.Read(request.Schema, ReaderSettings_ValidationEventHandler);
                readerSettings.Schemas.Add(xmlSchema);

                // Continue only if the validation are empty at this point
                if (!validationItems.Any())
                {
                    // Use the XML Reader vor parsing the xml file
                    using (var reader = XmlReader.Create(request.XmlContent, readerSettings))
                    {
                        while (await reader.ReadAsync())
                        {
                            // Just let the reader read each item..
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FeatureException("Schema validation failed", ex);
            }

            bool isPassed = false;
            if (validationItems != null)
            {
                if (validationItems.Any())
                {
                    isPassed = false;
                }
                else
                {
                    isPassed = true;
                }
            }

            result = new SchemaCompareResult(isPassed, validationItems);
            return result;
        }

        private void ReaderSettings_ValidationEventHandler(object? sender, ValidationEventArgs e)
        {
            if (validationItems != null)
            {
                validationItems.Add(new SchemaValidationItem(e.Severity.ToString(), e.Message));
            }
        }
    }
}

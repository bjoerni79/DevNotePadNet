using DevNotePad.Features;
using DevNotePad.Features.Xml;
using DevNotePad.MVVM;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace DevNotePad.ViewModel
{
    public class XmlSchemaValidatorViewModel : MainViewUiViewModel
    {
        public XmlSchemaValidatorViewModel()
        {
            Validate = new DefaultCommand(OnValidate);

            SchemaFile = @"D:\temp\test files\bookstore.xsd";
        }

        /*
         * see also : https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-6.0#xmlreader_nodes
         * via IXmlSchemaValidator interface
         * 
         * 
         * - Import from text
         * - Export to text
         * - Read from text file
         * 
         * - Import schema file
         * - Import
         * 
         * - Log? 
         * 
         * - Clear All
         */

        public string? SchemaFile { get; set; }

        public string? XmlContent { get; set; }

        public string? Result { get; set; }

        public IRefreshCommand? ImportFromText { get; private set; }

        public IRefreshCommand? ExportToText { get; private set; }

        public IRefreshCommand? ReadFromFile { get; private set; }

        public IRefreshCommand? Validate { get; private set; }

        public IRefreshCommand? Clear { get; private set; }

        private void OnValidate()
        {
            var schemaValidator = FeatureFactory.CreateXmlSchemaValidator();

            try
            {
                using (var schemaFileReader = File.OpenText(SchemaFile))
                using (var xmlContent = new StringReader(XmlContent))
                {
                    var request = new SchemaCompareRequest(xmlContent, schemaFileReader);
                    var validateTask = Task.Run(() => schemaValidator.CompareAsync(request));

                    validateTask.Wait();
                    var result = validateTask.Result;

                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("Is Valid : {0}\n", result.IsPassed);

                    if (result.ValidationItems != null)
                    {
                        foreach (var item in result.ValidationItems)
                        {
                            stringBuilder.AppendFormat("- {0}: {1}", item.Category, item.Description);
                        }
                    }

                    Result = stringBuilder.ToString();
                    RaisePropertyChange("Result");
                }
            }
            catch (AggregateException aEx)
            {
                foreach (var inner in aEx.InnerExceptions)
                {
                    var schemaException = inner as XmlSchemaException;
                    if (schemaException != null)
                    {
                        Result = schemaException.Message;
                    }

                    var xmlException = inner as XmlException;
                    if (xmlException != null)
                    {
                        Result = xmlException.Message;
                    }

                    if (schemaException == null && xmlException == null)
                    {
                        Result = "Unknown Error " + inner.Message;
                    }

                    RaisePropertyChange("Result");
                }
            }
            
        }
    }
}

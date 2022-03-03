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
            LoadXml = new DefaultCommand(OnLoadXml);
            ImportFromText = new DefaultCommand(OnImportFromText);
            Clear = new DefaultCommand(OnClear);
            AddSchemaFile = new DefaultCommand(OnAddSchemaFile);
        }

        public readonly string FileDialogFilterXmlSchema = "All|*.*|Schema XML|*.xsd;";

        public readonly string FileDialogFilterXml = "All|*.*|XML|*.xml;";

        public string? SchemaFile { get; set; }

        public string? XmlContent { get; set; }

        public string? Result { get; set; }

        public IRefreshCommand? ImportFromText { get; private set; }

        public IRefreshCommand? LoadXml { get; private set; }

        public IRefreshCommand? AddSchemaFile { get; private set; }

        public IRefreshCommand? Validate { get; private set; }

        public IRefreshCommand? Clear { get; private set; }

        private void OnClear()
        {
            SchemaFile = null;
            XmlContent = null;
            Result = null;

            RaisePropertyChange("SchemaFile");
            RaisePropertyChange("XmlContent");
            RaisePropertyChange("Result");
        }

        private void OnAddSchemaFile()
        {
            var dialogService = ServiceHelper.GetDialogService();

            var result = dialogService.ShowOpenFileNameDialog(FileDialogFilterXmlSchema, dialog.GetCurrentWindow());
            if (result.IsConfirmed)
            {
                var file = result.File;
                SchemaFile = file;
                RaisePropertyChange("SchemaFile");
            }
        }

        private void OnLoadXml()
        {
            //Load the XML from the file and copy it to the XmlContent
            var dialogService = ServiceHelper.GetDialogService();

            var result = dialogService.ShowOpenFileNameDialog(FileDialogFilterXml, dialog.GetCurrentWindow());
            if (result.IsConfirmed)
            {
                var file = result.File;

                try
                {
                    using (var stream = File.OpenText(file))
                    {
                        XmlContent = stream.ReadToEnd();
                        RaisePropertyChange("XmlContent");
                    }
                }
                catch (Exception ex)
                {
                    Result = ex.Message;
                    RaisePropertyChange("Result");
                }
            }
        }

        private void OnImportFromText()
        {
            var contentFromText = textComponent.GetText(false);
            XmlContent = contentFromText;
            RaisePropertyChange("XmlContent");
        }

        private void OnValidate()
        {
            var schemaValidator = FeatureFactory.CreateXmlSchemaValidator();
            var runValidation = true;

            if (string.IsNullOrEmpty(SchemaFile))
            {
                Result = "Invalid Schema File detected";
                RaisePropertyChange("Result");
                runValidation = false;
            }

            if (string.IsNullOrEmpty(XmlContent))
            {
                Result = "No Xml Content available to validate";
                RaisePropertyChange("Result");
                runValidation= false;
            }

            if (runValidation && !File.Exists(SchemaFile))
            {
                Result = "Schema File cannot be found";
                RaisePropertyChange("Result");
                runValidation = false;
            }

            if (runValidation)
            {
                try
                {
                    using (var schemaFileReader = File.OpenText(SchemaFile))
                    using (var xmlContent = new StringReader(XmlContent))
                    {
                        var request = new SchemaCompareRequest(xmlContent, schemaFileReader);
                        var validateTask = Task.Run(() => schemaValidator.CompareAsync(request));

                        // Wait for the result and continue with the result later
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
                        var featureException = inner as FeatureException;
                        if (featureException != null)
                        {
                            Result = featureException.Message + "\n" + featureException.Details;
                        }
                        else
                        {
                            Result = inner.Message;
                        }

                        RaisePropertyChange("Result");
                    }
                }
            }
        }
    }
}

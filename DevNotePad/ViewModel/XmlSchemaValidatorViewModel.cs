using CommunityToolkit.Mvvm.Input;
using DevNotePad.Features;
using DevNotePad.Features.Xml;
using DevNotePad.MVVM;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class XmlSchemaValidatorViewModel : MainViewUiViewModel
    {
        public XmlSchemaValidatorViewModel()
        {
            Validate = new RelayCommand(OnValidate);
            LoadXml = new RelayCommand(OnLoadXml);
            ImportFromText = new RelayCommand(OnImportFromText);
            Clear = new RelayCommand(OnClear);
            AddSchemaFile = new RelayCommand(OnAddSchemaFile);

        }

        public string? SchemaFile { get; set; }

        public string? XmlContent { get; set; }

        public string? Result { get; set; }

        public RelayCommand? ImportFromText { get; private set; }

        public RelayCommand? LoadXml { get; private set; }

        public RelayCommand? AddSchemaFile { get; private set; }

        public RelayCommand? Validate { get; private set; }

        public RelayCommand? Clear { get; private set; }

        private void OnClear()
        {
            SchemaFile = null;
            XmlContent = null;
            Result = null;

            OnPropertyChanged("SchemaFile");
            OnPropertyChanged("XmlContent");
            OnPropertyChanged("Result");
        }

        private void OnAddSchemaFile()
        {
            var dialogService = ServiceHelper.GetDialogService();

            var result = dialogService.ShowOpenFileNameDialog(XmlToolHelper.FileDialogFilterXmlSchema, dialog.GetCurrentWindow());
            if (result.IsConfirmed)
            {
                var file = result.File;
                SchemaFile = file;
                OnPropertyChanged("SchemaFile");
            }
        }

        private void OnLoadXml()
        {
            try
            {
                var fileContent = xmlToolHelper.LoadXmlFromDialog();
                if (fileContent != null)
                {
                    XmlContent = fileContent;
                    OnPropertyChanged("XmlContent");
                }
            }
            catch (Exception ex)
            {
                Result = ex.Message;
                OnPropertyChanged("Result");
            }
        }

        private void OnImportFromText()
        {
            var contentFromText = textComponent.GetText(false);
            XmlContent = contentFromText;
            OnPropertyChanged("XmlContent");
        }

        private void OnValidate()
        {
            var schemaValidator = FeatureFactory.CreateXmlSchemaValidator();
            var runValidation = true;

            if (string.IsNullOrEmpty(SchemaFile))
            {
                Result = "Invalid Schema File detected";
                OnPropertyChanged("Result");
                runValidation = false;
            }

            if (string.IsNullOrEmpty(XmlContent))
            {
                Result = "No Xml Content available to validate";
                OnPropertyChanged("Result");
                runValidation = false;
            }

            if (runValidation && !File.Exists(SchemaFile))
            {
                Result = "Schema File cannot be found";
                OnPropertyChanged("Result");
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
                        OnPropertyChanged("Result");
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

                        OnPropertyChanged("Result");
                    }
                }
            }
        }
    }
}

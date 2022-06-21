using CommunityToolkit.Mvvm.Input;
using DevNotePad.Features;
using DevNotePad.Features.Xml;
using DevNotePad.MVVM;
using System;
using System.IO;
using System.Xml;

namespace DevNotePad.ViewModel
{
    public class XsltTransformerViewModel : MainViewUiViewModel
    {
        public XsltTransformerViewModel()
        {
            LoadXml = new RelayCommand(OnLoadXml);
            ImportFromText = new RelayCommand(OnImportFromText);
            Clear = new RelayCommand(OnClear);
            AddTransformationFile = new RelayCommand(OnAddTransformationFile);
            Transform = new RelayCommand(OnTransform);
        }

        public string? TransformationFile { get; set; }

        public string? XmlContent { get; set; }

        public string? Result { get; set; }

        public bool EnableScript { get; set; }

        public bool EnableDocumentFunction { get; set; }

        public RelayCommand? ImportFromText { get; private set; }

        public RelayCommand? LoadXml { get; private set; }

        public RelayCommand? AddTransformationFile { get; private set; }

        public RelayCommand? Transform { get; private set; }

        public RelayCommand? Clear { get; private set; }

        private void OnClear()
        {
            TransformationFile = null;
            XmlContent = null;
            Result = null;

            OnPropertyChanged("TransformationFile");
            OnPropertyChanged("XmlContent");
            OnPropertyChanged("Result");
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

        private void OnAddTransformationFile()
        {
            var dialogService = ServiceHelper.GetDialogService();

            var result = dialogService.ShowOpenFileNameDialog(XmlToolHelper.FilterDialogXmlXslt, dialog.GetCurrentWindow());
            if (result.IsConfirmed)
            {
                var file = result.File;
                TransformationFile = file;
                OnPropertyChanged("TransformationFile");
            }
        }

        private void OnTransform()
        {
            var runValidation = true;

            if (String.IsNullOrEmpty(TransformationFile))
            {
                Result = "Please provide a XSLT File";
                runValidation = false;
            }

            if (runValidation && String.IsNullOrEmpty(XmlContent))
            {
                Result = "Please provide the XML content";
            }

            if (runValidation)
            {
                var xsltService = FeatureFactory.CreateXsltTransformer();
                XSltTransformationResponse? response = null;

                var parameter = new XsltParameter(EnableScript, EnableDocumentFunction);

                try
                {
                    // Read the XML Content as TextReader...
                    using (var textReader = new StringReader(XmlContent))
                    using (var fileStream = File.OpenText(TransformationFile))
                    using (var xmlReader = XmlReader.Create(fileStream))
                    {
                        var request = new XSltTransformationRequest(textReader, xmlReader, parameter);
                        response = xsltService.Transform(request);
                    }
                }
                catch (FeatureException featureException)
                {
                    Result = featureException.BuildReport();
                }

                if (response != null)
                {
                    if (response.IsPassed)
                    {
                        Result = response.Result;
                    }
                    else
                    {
                        Result = "Internal Error.";
                    }
                }
            }

            OnPropertyChanged("Result");
        }
    }
}

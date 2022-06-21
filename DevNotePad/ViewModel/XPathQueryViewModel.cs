using CommunityToolkit.Mvvm.Input;
using DevNotePad.Features;
using DevNotePad.Features.Xml;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class XPathQueryViewModel : MainViewUiViewModel
    {
        public XPathQueryViewModel()
        {
            Clear = new RelayCommand(OnClear);
            LoadXml = new RelayCommand(OnLoadXml);
            ImportFromText = new RelayCommand(OnImportFromText);
            Run = new RelayCommand(OnRun);
        }

        public string? Query { get; set; }

        public string? XmlContent { get; set; }

        public string? Result { get; set; }

        public RelayCommand? Run { get; private set; }

        public RelayCommand? ImportFromText { get; private set; }

        public RelayCommand? LoadXml { get; private set; }

        public RelayCommand? Clear { get; private set; }

        private void OnClear()
        {
            Query = null;
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

        private void OnRun()
        {
            var runValidation = true;

            // Check if XPath query is present
            if (String.IsNullOrEmpty(Query))
            {
                Result = "Error: Please provide a XPath query";
                runValidation = false;
            }

            // Check if XML data is present
            if (runValidation && String.IsNullOrEmpty(XmlContent))
            {
                Result = "Error: Please provide XML";
                runValidation= false;
            }

            if (runValidation)
            {
                try
                {
                    using (var stringReader = new StringReader(XmlContent))
                    {
                        var xpathQueryService = FeatureFactory.CreateXmlXpathQuery();
                        var request = new XPathQueryRequest(stringReader, Query);

                        var response = xpathQueryService.Query(request);
                        if (response.IsMatching)
                        {
                            Result = response.Result;
                        }
                    }
                }
                catch (FeatureException featureEx)
                {
                    Result = featureEx.Message + "\n" + featureEx.Details;
                }
                catch (Exception ex)
                {
                    Result = ex.Message;
                }

            }

            OnPropertyChanged("Result");
        }
    }
}

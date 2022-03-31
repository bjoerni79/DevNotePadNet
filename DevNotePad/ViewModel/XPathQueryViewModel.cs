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
            Clear = new DefaultCommand(OnClear);
            LoadXml = new DefaultCommand(OnLoadXml);
            ImportFromText = new DefaultCommand(OnImportFromText);
            Run = new DefaultCommand(OnRun);
        }

        public string? Query { get; set; }

        public string? XmlContent { get; set; }

        public string? Result { get; set; }

        public IRefreshCommand? Run { get; private set; }

        public IRefreshCommand? ImportFromText { get; private set; }

        public IRefreshCommand? LoadXml { get; private set; }

        public IRefreshCommand? Clear { get; private set; }

        private void OnClear()
        {
            Query = null;
            XmlContent = null;
            Result = null;

            RaisePropertyChange("TransformationFile");
            RaisePropertyChange("XmlContent");
            RaisePropertyChange("Result");
        }

        private void OnLoadXml()
        {
            try
            {
                var fileContent = xmlToolHelper.LoadXmlFromDialog();
                if (fileContent != null)
                {
                    XmlContent = fileContent;
                    RaisePropertyChange("XmlContent");
                }
            }
            catch (Exception ex)
            {
                Result = ex.Message;
                RaisePropertyChange("Result");
            }
        }

        private void OnImportFromText()
        {
            var contentFromText = textComponent.GetText(false);
            XmlContent = contentFromText;
            RaisePropertyChange("XmlContent");
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

            RaisePropertyChange("Result");
        }
    }
}

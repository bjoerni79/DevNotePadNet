using Generic.MVVM;
using System;
using System.Collections.Generic;
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
        }

        public string? Query { get; set; }

        public string? XmlContent { get; set; }

        public string? Result { get; set; }

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
    }
}

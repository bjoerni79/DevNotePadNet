using DevNotePad.MVVM;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class XsltTransformerViewModel : MainViewUiViewModel
    {
        public XsltTransformerViewModel()
        {
            LoadXml = new DefaultCommand(OnLoadXml);
            ImportFromText = new DefaultCommand(OnImportFromText);
            Clear = new DefaultCommand(OnClear);
            AddTransformationFile = new DefaultCommand(OnAddTransformationFile);
            Transform = new DefaultCommand(OnTransform);
        }

        public string? TransformationFile { get; set; }

        public string? XmlContent { get; set; }

        public string? Result { get; set; }

        public IRefreshCommand? ImportFromText { get; private set; }

        public IRefreshCommand? LoadXml { get; private set; }

        public IRefreshCommand? AddTransformationFile { get; private set; }

        public IRefreshCommand? Transform { get; private set; }

        public IRefreshCommand? Clear { get; private set; }

        private void OnClear()
        {
            TransformationFile = null;
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

        private void OnAddTransformationFile()
        {
            var dialogService = ServiceHelper.GetDialogService();

            var result = dialogService.ShowOpenFileNameDialog(XmlToolHelper.FilterDialogXmlXslt, dialog.GetCurrentWindow());
            if (result.IsConfirmed)
            {
                var file = result.File;
                TransformationFile = file;
                RaisePropertyChange("TransformationFile");
            }
        }

        private void OnTransform()
        {
            //TODO
        }
    }
}

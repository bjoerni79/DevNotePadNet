using DevNotePad.MVVM;
using DevNotePad.Shared.Dialog;
using System.IO;

namespace DevNotePad.ViewModel
{
    /// <summary>
    /// Collection of internal methods used in the XML tools 
    /// </summary>
    public class XmlToolHelper
    {
        private IDialog dialogInstance;

        public XmlToolHelper(IDialog dialog)
        {
            dialogInstance = dialog;
        }


        public static string FileDialogFilterXmlSchema = "All|*.*|Schema XML|*.xsd;";

        public static string FileDialogFilterXml = "All|*.*|XML|*.xml;";

        public static string FilterDialogXmlXslt = "All|*.*|XSLT|*.xslt;";

        /// <summary>
        /// Helper method: Opens a file dialog and returns the content of the text if available.
        /// </summary>
        /// <returns>Null if cancelled or the content</returns>
        public string? LoadXmlFromDialog()
        {
            return LoadXmlFromDialog(FileDialogFilterXml);
        }

        /// <summary>
        /// Helper method: Opens a file dialog and returns the content of the text if available.
        /// </summary>
        /// <param name="extensionFilter">the filter pattern for the OpenFile Dialog</param>
        /// <returns>Null if cancelled or the content</returns>
        public string? LoadXmlFromDialog(string extensionFilter)
        {
            //Load the XML from the file and copy it to the XmlContent
            var dialogService = ServiceHelper.GetDialogService();
            string? xmlContent = null;

            var result = dialogService.ShowOpenFileNameDialog(extensionFilter);
            if (result.IsConfirmed)
            {
                var file = result.File;

                using (var stream = File.OpenText(file))
                {
                    xmlContent = stream.ReadToEnd();
                }
            }

            return xmlContent;
        }
    }
}

using DevNotePad.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Service
{
    public interface IDialogService
    {
        DialogReturnValue ShowOpenFileNameDialog(string title, string defaultExtension, string searchPattern);

        DialogReturnValue ShowSaveFileDialog();

        void ShowErrorDialog(Exception ex, string component);

        void ShowWarningDialog(string warning, string component);

        bool ShowConfirmationDialog(string question, string title);

        bool ShowConfirmationDialog(string question, string title,string okButtonText);

        void OpenFindDialog(IMainViewUi ui);

        void OpenReplaceDialog(IMainViewUi ui);
    }
}

using DevNotePad.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevNotePad.Service
{
    public interface IDialogService
    {
        DialogReturnValue ShowOpenFileNameDialog(string defaultExtension);

        DialogReturnValue ShowOpenFileNameDialog(string defaultExtension, Window owner);

        DialogReturnValue ShowSaveFileDialog(string defaultExtension);

        DialogReturnValue ShowSaveFileDialog(string defaultExtension, Window owner);

        void ShowErrorDialog(Exception ex, string component, Window owner);

        void ShowErrorDialog(Exception ex, string component);

        void ShowWarningDialog(string warning, string component, Window owner);

        void ShowWarningDialog(string warning, string component);

        bool ShowConfirmationDialog(string question, string title);

        bool ShowConfirmationDialog(string question, string title,string okButtonText);

        void OpenFindDialog(IMainViewUi ui, ITextComponent textComponent);

        void OpenReplaceDialog(IMainViewUi ui, ITextComponent textComponent);

        void OpenBase64Dialog(IMainViewUi ui, ITextComponent textComponent);
    }
}

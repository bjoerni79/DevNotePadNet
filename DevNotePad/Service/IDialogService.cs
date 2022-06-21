using System;
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

        bool ShowConfirmationDialog(string question, string title, string okButtonText);

        void ShowSettings();
    }
}

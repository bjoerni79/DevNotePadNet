using System;
using System.Windows;

namespace DevNotePad.Service
{
    public interface IDialogService
    {
        DialogReturnValue ShowOpenFileNameDialog(string defaultExtension);

        DialogReturnValue ShowSaveFileDialog(string defaultExtension);

        void ShowErrorDialog(Exception ex, string component);

        void ShowWarningDialog(string warning, string component);

        bool ShowConfirmationDialog(string question, string title);

        bool ShowConfirmationDialog(string question, string title, string okButtonText);

        void ShowSettings();
    }
}

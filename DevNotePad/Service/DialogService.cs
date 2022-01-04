using DevNotePad.Shared;
using DevNotePad.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevNotePad.Service
{
    internal class DialogService : IDialogService
    {
        private Window owner;

        // ShowDialog does not work for the Find and Replace dialog due to a selection/focus issue. 
        // This is a work around, which simulates the ShowDialog method call
        private FindDialog? currentFindDialog;
        private ReplaceDialog? currentReplaceDialog;
        private FindDialogViewModel? findDialogViewModel;
        private ReplaceDialogViewModel? replaceDialogViewModel;

        internal DialogService(Window owner)
        {
            this.owner = owner;
        }

        public void OpenFindDialog(IMainViewUi ui)
        {
            if (ui == null)
            {
                throw new ArgumentNullException(nameof(ui));
            }

            if (currentFindDialog == null)
            {
                currentFindDialog = new FindDialog() { Owner = owner};
                findDialogViewModel = new FindDialogViewModel(ui, currentFindDialog);
                currentFindDialog.DataContext = findDialogViewModel;
            }

            currentFindDialog.Show();
        }

        public void OpenReplaceDialog(IMainViewUi ui)
        {
            if (ui == null)
            {
                throw new ArgumentNullException(nameof(ui));
            }

            if (currentReplaceDialog == null)
            {
                currentReplaceDialog = new ReplaceDialog() { Owner = owner};
                replaceDialogViewModel = new ReplaceDialogViewModel(ui, currentReplaceDialog);
            }

            currentReplaceDialog.Show();
        }

        public bool ShowConfirmationDialog(string question, string title)
        {
            return ShowConfirmationDialog(question, title, "OK");
        }

        public bool ShowConfirmationDialog(string question, string title, string okButtonText)
        {
            var confirmDialog = new ConfirmDialog() { Owner = owner };
            confirmDialog.Init(question, title,okButtonText);

            var result = confirmDialog.ShowDialog();
            if (result.HasValue)
            {
                return result.Value;
            }

            return false;
        }

        public void ShowErrorDialog(Exception ex, string component)
        {
            var message = ex.Message;
            var dialogTitle = "Error";
            var errorDialog = new OkDialog();
            errorDialog.Owner = owner;
            errorDialog.Init(message, component, dialogTitle);

            errorDialog.ShowDialog();
        }

        public DialogReturnValue ShowOpenFileNameDialog(string title, string defaultExtension, string searchPattern)
        {
            var openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog(owner);

            if (result.HasValue && result.Value)
            {
                return new DialogReturnValue(true, openFileDialog.FileName);
            }

            return new DialogReturnValue(false, String.Empty);
        }

        public DialogReturnValue ShowSaveFileDialog()
        {
            var saveFileDialog = new SaveFileDialog();
            var result = saveFileDialog.ShowDialog(owner);

            if (result.HasValue && result.Value)
            {
                return new DialogReturnValue(true, saveFileDialog.FileName);
            }

            return new DialogReturnValue(false, String.Empty);
        }

        public void ShowWarningDialog(string warning, string component)
        {
            //MessageBox.Show(warning, caption);

            var errorDialog = new OkDialog();
            errorDialog.Owner = owner;

            errorDialog.Init(warning, component, "Warning");
            errorDialog.ShowDialog();
        }

        
    }
}

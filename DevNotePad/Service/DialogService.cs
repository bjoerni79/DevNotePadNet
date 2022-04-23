using DevNotePad.Features;
using DevNotePad.MVVM;
using DevNotePad.Shared;
using DevNotePad.Shared.Dialog;
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
        private Window defaultOwner;

        internal DialogService(Window owner)
        {
            this.defaultOwner = owner;
        }

        public bool ShowConfirmationDialog(string question, string title)
        {
            return ShowConfirmationDialog(question, title, "OK");
        }

        public bool ShowConfirmationDialog(string question, string title, string okButtonText)
        {
            var confirmDialog = new ConfirmDialog() { Owner = defaultOwner,Topmost= true };
            confirmDialog.Init(question, title,okButtonText);

            var result = confirmDialog.ShowDialog();
            if (result.HasValue)
            {
                return result.Value;
            }

            return false;
        }

        public void ShowErrorDialog(Exception ex, string component, Window owner)
        {
            var message = ex.Message;
            var dialogTitle = "Error";
            var errorDialog = new OkDialog();
            errorDialog.Owner = owner;

            var featureException = ex as FeatureException;
            if (featureException != null)
            {
                errorDialog.Init(message, component, dialogTitle, featureException.Details);
            }
            else
            {
                errorDialog.Init(message, component, dialogTitle);
            }

            errorDialog.ShowDialog();
        }

        public void ShowErrorDialog(Exception ex, string component)
        {
            ShowErrorDialog(ex, component, defaultOwner);
        }

        public DialogReturnValue ShowOpenFileNameDialog(string defaultExtension)
        {
            return ShowOpenFileNameDialog(defaultExtension, defaultOwner);
        }

        public DialogReturnValue ShowOpenFileNameDialog(string defaultExtension, Window owner)
        {
            var openFileDialog = new OpenFileDialog() { Filter = defaultExtension, DefaultExt = "*.txt" };
            var result = openFileDialog.ShowDialog(owner);

            if (result.HasValue && result.Value)
            {
                return new DialogReturnValue(true, openFileDialog.FileName);
            }

            return new DialogReturnValue(false, String.Empty);
        }

        public DialogReturnValue ShowSaveFileDialog(string defaultExtension)
        {
            return ShowSaveFileDialog(defaultExtension, defaultOwner);
        }

        public DialogReturnValue ShowSaveFileDialog(string defaultExtension, Window owner)
        {
            //var saveFileDialog = new SaveFileDialog() { Filter = defaultExtension, DefaultExt = "*.txt" };
            var saveFileDialog = new SaveFileDialog() { Filter = defaultExtension};
            var result = saveFileDialog.ShowDialog(owner);

            if (result.HasValue && result.Value)
            {
                return new DialogReturnValue(true, saveFileDialog.FileName);
            }

            return new DialogReturnValue(false, String.Empty);
        }

        public void ShowSettings()
        {
            //TODO: Get the ViewModel or create one

            //TODO: Clean up the old settings UI if available

            //TODO: Create a new view and associate the VM

            //TODO: Start the dialog as Modal!
        }

        public void ShowWarningDialog(string warning, string component)
        {
            ShowWarningDialog(warning, component, defaultOwner);
        }

        public void ShowWarningDialog(string warning, string component, Window owner)
        {
            var errorDialog = new OkDialog();
            errorDialog.Owner = owner;

            errorDialog.Init(warning, component, "Warning");
            errorDialog.ShowDialog();
        }

    }
}

using DevNotePad.Features;
using DevNotePad.MVVM;
using DevNotePad.Shared.Dialog;
using DevNotePad.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;

namespace DevNotePad.Service
{
    internal class DialogService : IDialogService
    {

        private SettingsDialog? currentSettingsDialogView;

        internal DialogService()
        {
        }

        public bool ShowConfirmationDialog(string question, string title)
        {
            return ShowConfirmationDialog(question, title, "OK");
        }

        public bool ShowConfirmationDialog(string question, string title, string okButtonText)
        {
            var confirmDialog = new ConfirmDialog() { Topmost = true };
            confirmDialog.Init(question, title, okButtonText);

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

        public DialogReturnValue ShowOpenFileNameDialog(string defaultExtension )
        {
            var openFileDialog = new OpenFileDialog() { Filter = defaultExtension, DefaultExt = "*.txt" };
            var result = openFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                return new DialogReturnValue(true, openFileDialog.FileName);
            }

            return new DialogReturnValue(false, String.Empty);
        }

        public DialogReturnValue ShowSaveFileDialog(string defaultExtension)
        {
            //var saveFileDialog = new SaveFileDialog() { Filter = defaultExtension, DefaultExt = "*.txt" };
            var saveFileDialog = new SaveFileDialog() { Filter = defaultExtension };
            var result = saveFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                return new DialogReturnValue(true, saveFileDialog.FileName);
            }

            return new DialogReturnValue(false, String.Empty);
        }

        public void ShowSettings()
        {
            var facade = ServiceHelper.GetFacade();
            SettingsDialogViewModel vm;

            // Get the ViewModel or create one
            var isVmAvailable = facade.Exists(ViewModelInstances.SettingsDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<SettingsDialogViewModel>(ViewModelInstances.SettingsDialog);
            }
            else
            {
                vm = new SettingsDialogViewModel();
                facade.AddUnique(vm, ViewModelInstances.SettingsDialog);
            }

            // Clean up the old settings UI if available
            if (currentSettingsDialogView != null)
            {
                currentSettingsDialogView.Close();
            }

            // Create a new view and associate the VM
            currentSettingsDialogView = new SettingsDialog();

            vm.Init(currentSettingsDialogView);
            currentSettingsDialogView.DataContext = vm;

            // Start the dialog as Modal!
            currentSettingsDialogView.Show();
        }

        public void ShowWarningDialog(string warning, string component)
        {
            var errorDialog = new OkDialog();

            errorDialog.Init(warning, component, "Warning");
            errorDialog.ShowDialog();
        }

    }
}

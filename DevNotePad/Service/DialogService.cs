using DevNotePad.Features;
using DevNotePad.MVVM;
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



        internal DialogService(Window owner)
        {
            this.owner = owner;
        }

        public void OpenFindDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (ui == null)
            {
                throw new ArgumentNullException(nameof(ui));
            }

            // Maintain only one view model independant from IMainViewUi and IDialog instance
            var facade = FacadeFactory.Create();

            FindDialogViewModel vm;
            var isVmAvailable = facade.Exists(Bootstrap.ViewModelFindDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<FindDialogViewModel>(Bootstrap.ViewModelFindDialog);
            }
            else
            {
                vm = new FindDialogViewModel();
                facade.AddUnique(vm, Bootstrap.ViewModelFindDialog);
            }
            
            // Workaround:  We have to close the view and open a new one. All data is stored in the view model
            if (currentFindDialog != null)
            {
                currentFindDialog.Close();
            }

            currentFindDialog = new FindDialog() { Owner = owner };
            currentFindDialog.DataContext = vm;
            vm.Init(ui, currentFindDialog, textComponent);

            currentFindDialog.Show();
        }

        public void OpenReplaceDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (ui == null)
            {
                throw new ArgumentNullException(nameof(ui));
            }

            // Maintain only one view model independant from IMainViewUi and IDialog instance
            var facade = FacadeFactory.Create();

            ReplaceDialogViewModel vm;
            var isVmAvailable = facade.Exists(Bootstrap.ViewModelReplaceDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<ReplaceDialogViewModel>(Bootstrap.ViewModelReplaceDialog);
            }
            else
            {
                vm = new ReplaceDialogViewModel();
                facade.AddUnique(vm, Bootstrap.ViewModelReplaceDialog);
            }

            // Workaround:  We have to close the view and open a new one. All data is stored in the view model
            if (currentReplaceDialog != null)
            {
                currentReplaceDialog.Close();
            }

            currentReplaceDialog = new ReplaceDialog() { Owner = owner };
            currentReplaceDialog.DataContext = vm;
            vm.Init(ui, currentReplaceDialog, textComponent);

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

        public DialogReturnValue ShowOpenFileNameDialog(string defaultExtension)
        {
            var openFileDialog = new OpenFileDialog() { Filter=defaultExtension, DefaultExt="*.txt"};
            var result = openFileDialog.ShowDialog(owner);

            if (result.HasValue && result.Value)
            {
                return new DialogReturnValue(true, openFileDialog.FileName);
            }

            return new DialogReturnValue(false, String.Empty);
        }

        public DialogReturnValue ShowSaveFileDialog(string defaultExtension)
        {
            var saveFileDialog = new SaveFileDialog() { Filter=defaultExtension, DefaultExt = "*.txt" };
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

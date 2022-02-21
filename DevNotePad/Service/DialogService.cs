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

        // ShowDialog does not work for the Find and Replace dialog due to a selection/focus issue. 
        // This is a work around, which simulates the ShowDialog method call
        private FindDialog? currentFindDialog;
        private ReplaceDialog? currentReplaceDialog;
        private Base64ToolDialog? currentBase64ToolDialog;
        private AppletToolDialog? currentAppletToolDialog;
        private XmlSchemaValidatorView? currentXmlSchemaValidatorView;

        internal DialogService(Window owner)
        {
            this.defaultOwner = owner;
        }

        public void OpenAppletToolDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (ui == null)
            {
                throw new ArgumentNullException(nameof(ui));
            }

            // Maintain only one view model independant from IMainViewUi and IDialog instance
            var facade = FacadeFactory.Create();

            AppletToolViewModel vm;
            var isVmAvailable = facade.Exists(Bootstrap.ViewModelAppletDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<AppletToolViewModel>(Bootstrap.ViewModelAppletDialog);
            }
            else
            {
                vm = new AppletToolViewModel();
                facade.AddUnique(vm, Bootstrap.ViewModelAppletDialog);
            }

            if (currentAppletToolDialog != null)
            {
                currentAppletToolDialog.Close();
            }

            currentAppletToolDialog = new AppletToolDialog();
            currentAppletToolDialog.DataContext = vm;

            vm.Init(ui, currentAppletToolDialog, textComponent);

            currentAppletToolDialog.Show();
        }

        public void OpenBase64Dialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (ui == null)
            {
                throw new ArgumentNullException(nameof(ui));
            }

            // Maintain only one view model independant from IMainViewUi and IDialog instance
            var facade = FacadeFactory.Create();

            Base64ToolViewModel vm;
            var isVmAvailable = facade.Exists(Bootstrap.ViewModelBase64Dialog);
            if (isVmAvailable)
            {
                vm = facade.Get<Base64ToolViewModel>(Bootstrap.ViewModelBase64Dialog);
            }
            else
            {
                vm = new Base64ToolViewModel();
                facade.AddUnique(vm, Bootstrap.ViewModelBase64Dialog);
            }

            if (currentBase64ToolDialog != null)
            {
                currentBase64ToolDialog.Close();
            }

            currentBase64ToolDialog = new Base64ToolDialog();
            currentBase64ToolDialog.DataContext = vm;

            vm.Init(ui, currentBase64ToolDialog, textComponent);

            currentBase64ToolDialog.Show();

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

            currentFindDialog = new FindDialog() { Owner = defaultOwner };
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

            currentReplaceDialog = new ReplaceDialog() { Owner = defaultOwner };
            currentReplaceDialog.DataContext = vm;
            vm.Init(ui, currentReplaceDialog, textComponent);

            currentReplaceDialog.Show();
        }

        public void OpenXmlSchemaValidatorDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (ui == null)
            {
                throw new ArgumentNullException(nameof(ui));
            }

            // Maintain only one view model independant from IMainViewUi and IDialog instance
            var facade = FacadeFactory.Create();

            XmlSchemaValidatorViewModel vm;
            var isVmAvailable = facade.Exists(Bootstrap.ViewModelXmlValidatorSchemaDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<XmlSchemaValidatorViewModel>(Bootstrap.ViewModelXmlValidatorSchemaDialog);
            }
            else
            {
                vm = new XmlSchemaValidatorViewModel();
                facade.AddUnique(vm, Bootstrap.ViewModelXmlValidatorSchemaDialog);
            }

            if (currentXmlSchemaValidatorView != null)
            {
                currentXmlSchemaValidatorView.Close();
            }

            currentXmlSchemaValidatorView = new XmlSchemaValidatorView();
            currentXmlSchemaValidatorView.DataContext = vm;

            vm.Init(ui, currentXmlSchemaValidatorView, textComponent);
            currentXmlSchemaValidatorView.Show();
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

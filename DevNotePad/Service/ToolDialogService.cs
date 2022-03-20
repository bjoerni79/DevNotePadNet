using DevNotePad.MVVM;
using DevNotePad.Shared.Dialog;
using DevNotePad.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevNotePad.Service
{
    internal class ToolDialogService : IToolDialogService
    {
        private Window defaultOwner;

        // ShowDialog does not work for the Find and Replace dialog due to a selection/focus issue. 
        // This is a work around, which simulates the ShowDialog method call
        private FindDialog? currentFindDialog;
        private ReplaceDialog? currentReplaceDialog;
        private Base64ToolDialog? currentBase64ToolDialog;
        private AppletToolDialog? currentAppletToolDialog;
        private XmlSchemaValidatorView? currentXmlSchemaValidatorView;
        private TreeView? currentTreeView;

        internal ToolDialogService(Window owner)
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

        public void OpenTreeView()
        {
            var facade = FacadeFactory.Create();

            TreeViewModel vm;
            var isVmAvailable = facade.Exists(Bootstrap.ViewModelTreeViewDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<TreeViewModel>(Bootstrap.ViewModelTreeViewDialog);
            }
            else
            {
                vm = new TreeViewModel();
                facade.AddUnique(vm, Bootstrap.ViewModelTreeViewDialog);
            }

            if (currentTreeView != null)
            {
                currentTreeView.Close();
            }

            currentTreeView = new TreeView();
            currentTreeView.DataContext = vm;

            vm.Init(currentTreeView);
            currentTreeView.Show();
        }
    }
}

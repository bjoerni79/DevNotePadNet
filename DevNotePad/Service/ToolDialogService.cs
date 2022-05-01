using DevNotePad.MVVM;
using DevNotePad.Shared.Dialog;
using DevNotePad.ViewModel;
using Generic.MVVM.IOC;
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
        private XmlXPathQueryView? currentXPathQueryView;
        private XsltTransformerView? currentXSltTransformerView;
        private RegularExpressionView? currentRegularExpressionView;
        private TreeView? currentTreeView;

        internal ToolDialogService(Window owner)
        {
            this.defaultOwner = owner;
        }

        public void OpenAppletToolDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            var facade = Init(ui);

            AppletToolViewModel vm;
            var isVmAvailable = facade.Exists(ViewModelInstances.AppletDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<AppletToolViewModel>(ViewModelInstances.AppletDialog);
            }
            else
            {
                vm = new AppletToolViewModel();
                facade.AddUnique(vm, ViewModelInstances.AppletDialog);
            }

            if (currentAppletToolDialog != null)
            {
                currentAppletToolDialog.Close();
            }

            currentAppletToolDialog = new AppletToolDialog();

            OpenDialog(ui, textComponent, vm, currentAppletToolDialog, currentAppletToolDialog);
        }

        public void OpenBase64Dialog(IMainViewUi ui, ITextComponent textComponent)
        {
            var facade = Init(ui);

            Base64ToolViewModel vm;
            var isVmAvailable = facade.Exists(ViewModelInstances.Base64Dialog);
            if (isVmAvailable)
            {
                vm = facade.Get<Base64ToolViewModel>(ViewModelInstances.Base64Dialog);
            }
            else
            {
                vm = new Base64ToolViewModel();
                facade.AddUnique(vm, ViewModelInstances.Base64Dialog);
            }

            if (currentBase64ToolDialog != null)
            {
                currentBase64ToolDialog.Close();
            }

            currentBase64ToolDialog = new Base64ToolDialog();
            OpenDialog(ui, textComponent,vm, currentBase64ToolDialog, currentBase64ToolDialog);
        }

        public void OpenFindDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            var facade = Init(ui);

            FindDialogViewModel vm;
            var isVmAvailable = facade.Exists(ViewModelInstances.FindDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<FindDialogViewModel>(ViewModelInstances.FindDialog);
            }
            else
            {
                vm = new FindDialogViewModel();
                facade.AddUnique(vm, ViewModelInstances.FindDialog);
            }

            // Workaround:  We have to close the view and open a new one. All data is stored in the view model
            if (currentFindDialog != null)
            {
                currentFindDialog.Close();
            }

            currentFindDialog = new FindDialog() { Owner = defaultOwner };

            OpenDialog(ui, textComponent, vm, currentFindDialog, currentFindDialog);
        }

        public void OpenReplaceDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            var facade = Init(ui);

            ReplaceDialogViewModel vm;
            var isVmAvailable = facade.Exists(ViewModelInstances.ReplaceDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<ReplaceDialogViewModel>(ViewModelInstances.ReplaceDialog);
            }
            else
            {
                vm = new ReplaceDialogViewModel();
                facade.AddUnique(vm, ViewModelInstances.ReplaceDialog);
            }

            // Workaround:  We have to close the view and open a new one. All data is stored in the view model
            if (currentReplaceDialog != null)
            {
                currentReplaceDialog.Close();
            }

            currentReplaceDialog = new ReplaceDialog() { Owner = defaultOwner };
            OpenDialog(ui, textComponent, vm, currentReplaceDialog, currentReplaceDialog);
        }

        public void OpenXmlSchemaValidatorDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            var facade = Init(ui);

            XmlSchemaValidatorViewModel vm;
            var isVmAvailable = facade.Exists(ViewModelInstances.XmlValidatorSchemaDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<XmlSchemaValidatorViewModel>(ViewModelInstances.XmlValidatorSchemaDialog);
            }
            else
            {
                vm = new XmlSchemaValidatorViewModel();
                facade.AddUnique(vm, ViewModelInstances.XmlValidatorSchemaDialog);
            }

            // Workaround:  If the view is still visible this is the only way of re-opening it. 
            if (currentXmlSchemaValidatorView != null)
            {
                currentXmlSchemaValidatorView.Close();
            }

            currentXmlSchemaValidatorView = new XmlSchemaValidatorView();
            OpenDialog(ui, textComponent, vm, currentXmlSchemaValidatorView, currentXmlSchemaValidatorView);
        }

        public void OpenXPathQueryDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            var facade = Init(ui);

            XPathQueryViewModel vm;
            var isVmAvailable = facade.Exists(ViewModelInstances.XmlXPath);
            if (isVmAvailable)
            {
                vm = facade.Get<XPathQueryViewModel>(ViewModelInstances.XmlXPath);
            }
            else
            {
                vm = new XPathQueryViewModel();
                facade.AddUnique(vm, ViewModelInstances.XmlXPath);
            }

            if (currentXPathQueryView != null)
            {
                currentXPathQueryView.Close();
            }

            currentXPathQueryView = new XmlXPathQueryView();
            OpenDialog(ui, textComponent, vm, currentXPathQueryView, currentXPathQueryView);
        }

        public void OpenXSltTransfomerDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            var facade = Init(ui);

            XsltTransformerViewModel vm;
            var isVmAvailable = facade.Exists(ViewModelInstances.XmlXSlt);
            if (isVmAvailable)
            {
                vm = facade.Get<XsltTransformerViewModel>(ViewModelInstances.XmlXSlt);
            }
            else
            {
                vm = new XsltTransformerViewModel();
                facade.AddUnique(vm, ViewModelInstances.XmlXSlt);
            }

            if (currentXSltTransformerView != null)
            {
                currentXSltTransformerView.Close();
            }

            currentXSltTransformerView = new XsltTransformerView();
            OpenDialog(ui, textComponent, vm, currentXSltTransformerView, currentXSltTransformerView);
        }

        public void OpenRegularExpressionDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            var facade = Init(ui);

            RegularExpressionViewModel vm;
            var isVmAvailable = facade.Exists(ViewModelInstances.RegularExpressionDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<RegularExpressionViewModel>(ViewModelInstances.RegularExpressionDialog);
            }
            else
            {
                vm = new RegularExpressionViewModel();
                facade.AddUnique(vm, ViewModelInstances.RegularExpressionDialog);
            }

            if (currentRegularExpressionView != null)
            {
                currentRegularExpressionView.Close();
            }

            currentRegularExpressionView = new RegularExpressionView();
            OpenDialog(ui, textComponent, vm, currentRegularExpressionView, currentRegularExpressionView);
        }

        public void OpenGuidDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            var facade = Init(ui);

            //TODO
        }

        private ContainerFacade Init(IMainViewUi ui)
        {
            if (ui == null)
            {
                throw new ArgumentNullException(nameof(ui));
            }

            // Maintain only one view model independant from IMainViewUi and IDialog instance
            var facade = FacadeFactory.Create();
            return facade;
        }

        /// <summary>
        /// Generic procedure for the tools dialog. Init the the view model and show the view.
        /// </summary>
        /// <param name="ui">the current main view</param>
        /// <param name="textComponent">the active text component</param>
        /// <param name="viewModel">the view model to connect</param>
        /// <param name="view">the view as Window type</param>
        /// <param name="dialogInstance">the view as IDialog type</param>
        private void OpenDialog(IMainViewUi ui, ITextComponent textComponent, MainViewUiViewModel viewModel, Window view,IDialog dialogInstance)
        {
            // Init the view model. The IDialog interface is the channel between the view model and the view.
            viewModel.Init(ui, dialogInstance, textComponent);

            // Set the context and show it.
            if (view != null)
            {
                view.DataContext = viewModel;
                view.Show();
            }
        }

        public void OpenTreeView()
        {
            var facade = FacadeFactory.Create();

            TreeViewModel vm;
            var isVmAvailable = facade.Exists(ViewModelInstances.TreeViewDialog);
            if (isVmAvailable)
            {
                vm = facade.Get<TreeViewModel>(ViewModelInstances.TreeViewDialog);
            }
            else
            {
                vm = new TreeViewModel();
                facade.AddUnique(vm, ViewModelInstances.TreeViewDialog);
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

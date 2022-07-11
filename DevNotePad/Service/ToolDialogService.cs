using DevNotePad.MVVM;
using DevNotePad.Shared.Dialog;
using DevNotePad.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace DevNotePad.Service
{
    public class ToolDialogService : IToolDialogService
    {

        // ShowDialog does not work for the Find and Replace dialog due to a selection/focus issue. 
        // This is a work around, which simulates the ShowDialog method call
        private FindDialog? currentFindDialog;
        private ReplaceDialog? currentReplaceDialog;
        private Base64ToolDialog? currentBase64ToolDialog;
        private XmlSchemaValidatorView? currentXmlSchemaValidatorView;
        private XmlXPathQueryView? currentXPathQueryView;
        private XsltTransformerView? currentXSltTransformerView;
        private RegularExpressionView? currentRegularExpressionView;
        private TreeView? currentTreeView;
        private GuidCreatorView? currentGuidCreatorView;

        public ToolDialogService()
        {
        }

        public void OpenBase64Dialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (currentBase64ToolDialog != null)
            {
                currentBase64ToolDialog.Close();
            }

            currentBase64ToolDialog = new Base64ToolDialog();
            var vm = App.Current.BootStrap.Services.GetService<Base64ToolViewModel>();
            OpenDialog(ui, textComponent, vm, currentBase64ToolDialog, currentBase64ToolDialog);
        }

        public void OpenFindDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (currentFindDialog != null)
            {
                currentFindDialog.Close();
            }

            currentFindDialog = new FindDialog();
            var vm = App.Current.BootStrap.Services.GetService<FindDialogViewModel>();
            OpenDialog(ui, textComponent, vm, currentFindDialog, currentFindDialog);
        }

        public void OpenReplaceDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (currentReplaceDialog != null)
            {
                currentReplaceDialog.Close();
            }

            currentReplaceDialog = new ReplaceDialog();
            var vm = App.Current.BootStrap.Services.GetService<ReplaceDialogViewModel>();
            OpenDialog(ui, textComponent, vm, currentReplaceDialog, currentReplaceDialog);
        }

        public void OpenXmlSchemaValidatorDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (currentXmlSchemaValidatorView != null)
            {
                currentXmlSchemaValidatorView.Close();
            }

            currentXmlSchemaValidatorView = new XmlSchemaValidatorView();
            var vm = App.Current.BootStrap.Services.GetService<XmlSchemaValidatorViewModel>();
            OpenDialog(ui, textComponent, vm, currentXmlSchemaValidatorView, currentXmlSchemaValidatorView);
        }

        public void OpenXPathQueryDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (currentXPathQueryView != null)
            {
                currentXPathQueryView.Close();
            }

            currentXPathQueryView = new XmlXPathQueryView();
            var vm = App.Current.BootStrap.Services.GetService<XPathQueryViewModel>();
            OpenDialog(ui, textComponent, vm, currentXPathQueryView, currentXPathQueryView);
        }

        public void OpenXSltTransfomerDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (currentXSltTransformerView != null)
            {
                currentXSltTransformerView.Close();
            }

            currentXSltTransformerView = new XsltTransformerView();
            var vm = App.Current.BootStrap.Services.GetService<XsltTransformerViewModel>();
            OpenDialog(ui, textComponent, vm, currentXSltTransformerView, currentXSltTransformerView);
        }

        public void OpenRegularExpressionDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (currentRegularExpressionView != null)
            {
                currentRegularExpressionView.Close();
            }

            currentRegularExpressionView = new RegularExpressionView();
            var vm = App.Current.BootStrap.Services.GetService<RegularExpressionViewModel>();
            OpenDialog(ui, textComponent, vm, currentRegularExpressionView, currentRegularExpressionView);
        }

        public void OpenGuidDialog(IMainViewUi ui, ITextComponent textComponent)
        {
            if (currentGuidCreatorView != null)
            {
                currentGuidCreatorView.Close();
            }

            currentGuidCreatorView = new GuidCreatorView();
            var vm = App.Current.BootStrap.Services.GetService<GuidCreatorViewModel>();
            OpenDialog(ui, textComponent, vm, currentGuidCreatorView, currentGuidCreatorView);
        }

        /// <summary>
        /// Generic procedure for the tools dialog. Init the the view model and show the view.
        /// </summary>
        /// <param name="ui">the current main view</param>
        /// <param name="textComponent">the active text component</param>
        /// <param name="viewModel">the view model to connect</param>
        /// <param name="view">the view as Window type</param>
        /// <param name="dialogInstance">the view as IDialog type</param>
        private void OpenDialog(IMainViewUi ui, ITextComponent textComponent, MainViewUiViewModel viewModel, Window view, IDialog dialogInstance)
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
            if (currentTreeView != null)
            {
                currentTreeView.Close();
            }

            var vm = App.Current.BootStrap.Services.GetService<TreeViewModel>();

            currentTreeView = new TreeView();
            currentTreeView.DataContext = vm;

            vm.Init(currentTreeView);
            currentTreeView.Show();
        }


    }
}

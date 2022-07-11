using DevNotePad.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for ConfirmDialog.xaml
    /// </summary>
    public partial class ConfirmDialog : Window, IDialog
    {
        public ConfirmDialog()
        {
            InitializeComponent();
        }

        public void Init(string question, string dialogTitle, string okButtonText)
        {
            var viewModel = App.Current.BootStrap.Services.GetService<ConfimDialogViewModel>();
            DataContext = viewModel;

            viewModel.Init(question, dialogTitle, okButtonText, this);
        }

        public void CloseDialog(bool confirmed)
        {
            if (confirmed)
            {
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }

            Close();
        }

        public void FocusDialog()
        {
            this.Focus();
        }

        public Window GetCurrentWindow()
        {
            return this;
        }
    }
}

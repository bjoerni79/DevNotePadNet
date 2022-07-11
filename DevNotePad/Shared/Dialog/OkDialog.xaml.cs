using DevNotePad.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for ErrorDialog.xaml
    /// </summary>
    public partial class OkDialog : Window, IDialog
    {
        public OkDialog()
        {
            InitializeComponent();

        }

        public void CloseDialog(bool confirmed)
        {
            DialogResult = true;
            Close();
        }

        public void Init(string message, string component, string title, string? details)
        {
            var vm = App.Current.BootStrap.Services.GetService<OkDialogViewModel>();
            vm.Init(message, component, title, this);
            vm.Details = details;

            DataContext = vm;
        }

        public void Init(string message, string component, string title)
        {
            Init(message, component, title, null);
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

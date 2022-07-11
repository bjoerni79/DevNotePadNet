using DevNotePad.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window, IDialog
    {
        public SettingsDialog()
        {
            InitializeComponent();

            var vm = App.Current.BootStrap.Services.GetService<SettingsDialogViewModel>();
            vm.Init(this);

            DataContext = vm;
        }

        public void CloseDialog(bool confirmed)
        {
            Close();
        }

        public Window GetCurrentWindow()
        {
            return this;
        }
    }
}

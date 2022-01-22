using DevNotePad.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DevNotePad.Shared
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
            var vm = new OkDialogViewModel();
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

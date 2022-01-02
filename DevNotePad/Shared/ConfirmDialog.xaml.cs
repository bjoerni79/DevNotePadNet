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
            var viewModel = new ConfimDialogViewModel();
            DataContext = viewModel;

            viewModel.Init(question, dialogTitle,okButtonText, this);
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
    }
}

using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for FindDialog.xaml
    /// </summary>
    public partial class FindDialog : Window, IDialog
    {
        public FindDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            //TODO: Apply the IDialog interface to the VM
        }

        public void CloseDialog(bool confirmed)
        {
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

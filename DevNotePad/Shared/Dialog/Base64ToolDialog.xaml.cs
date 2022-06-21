using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for Base64ToolDialog.xaml
    /// </summary>
    public partial class Base64ToolDialog : Window, IDialog
    {
        public Base64ToolDialog()
        {
            InitializeComponent();
        }

        public void CloseDialog(bool confirmed)
        {
            //DialogResult = confirmed;
            Close();
        }


        public Window GetCurrentWindow()
        {
            return this;
        }
    }
}

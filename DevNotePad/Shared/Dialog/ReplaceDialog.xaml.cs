using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for ReplaceDialog.xaml
    /// </summary>
    public partial class ReplaceDialog : Window, IDialog
    {
        public ReplaceDialog()
        {
            InitializeComponent();
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

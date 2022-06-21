using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for TreeView.xaml
    /// </summary>
    public partial class TreeView : Window, IDialog
    {
        public TreeView()
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

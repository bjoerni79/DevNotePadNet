using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for RegularExpressionView.xaml
    /// </summary>
    public partial class RegularExpressionView : Window, IDialog
    {
        public RegularExpressionView()
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

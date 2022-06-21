using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for XsltTransformerView.xaml
    /// </summary>
    public partial class XsltTransformerView : Window, IDialog
    {
        public XsltTransformerView()
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

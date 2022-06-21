using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for XmlSchemaValidatorView.xaml
    /// </summary>
    public partial class XmlSchemaValidatorView : Window, IDialog
    {
        public XmlSchemaValidatorView()
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

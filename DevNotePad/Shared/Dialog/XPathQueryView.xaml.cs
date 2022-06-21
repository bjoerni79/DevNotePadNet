using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for XmlXPathQueryView.xaml
    /// </summary>
    public partial class XmlXPathQueryView : Window, IDialog
    {
        public XmlXPathQueryView()
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

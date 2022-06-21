using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for GuidCreatorView.xaml
    /// </summary>
    public partial class GuidCreatorView : Window, IDialog
    {
        public GuidCreatorView()
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

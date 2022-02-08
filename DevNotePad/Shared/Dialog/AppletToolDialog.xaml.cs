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

namespace DevNotePad.Shared.Dialog
{
    /// <summary>
    /// Interaction logic for AppletToolDialog.xaml
    /// </summary>
    public partial class AppletToolDialog : Window, IDialog
    {
        public AppletToolDialog()
        {
            InitializeComponent();
        }

        private void OnCopyComponent(object sender, RoutedEventArgs e)
        {
            var item = componentView.SelectedItem as AppletComponentViewItem;
            if (item != null)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Title : {0}\n", item.Title);
                sb.AppendFormat("ID : {0}\n", item.Id);
                sb.AppendFormat("Content:\n{0}", item.Content);
                Clipboard.SetText(sb.ToString());
            }
        }

        public void CloseDialog(bool confirmed)
        {
            Close();
        }

        public void FocusDialog()
        {
            // Empty
        }

        public Window GetCurrentWindow()
        {
            return this;
        }
    }
}

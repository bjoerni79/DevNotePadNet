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

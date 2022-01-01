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

namespace DevNotePad.Shared
{
    /// <summary>
    /// Interaction logic for FindDialog.xaml
    /// </summary>
    public partial class FindDialog : Window, IDialog
    {
        public FindDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            //TODO: Apply the IDialog interface to the VM
        }

        public void CloseDialog(bool confirmed)
        {
            Close();
        }
    }
}

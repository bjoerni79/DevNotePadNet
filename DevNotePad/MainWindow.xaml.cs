using DevNotePad.MVVM;
using DevNotePad.Service;
using Generic.MVVM.Event;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevNotePad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var facade = FacadeFactory.Create();
            if (facade != null)
            {
                //var eventController = facade.Get<EventController>(Bootstrap.EventControllerId);

                IDialogService dialogService = new DialogService(this);
                facade.AddUnique(dialogService,Bootstrap.DialogServiceId);
            }
        }
    }
}

using DevNotePad.ViewModel;
using Microsoft.Extensions.DependencyInjection;
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

namespace DevNotePad.UI
{
    /// <summary>
    /// Interaction logic for Notifier.xaml
    /// </summary>
    public partial class Notifier : UserControl, INotifierView
    {
        public Notifier()
        {
            InitializeComponent();

            // Get a view model from the IoC container and establish the link between the view and vm
            var vm = App.Current.BootStrap.Services.GetService<NotifierViewModel>();
            if (vm != null)
            {
                vm.RegisterView(this);
                DataContext = vm;
            }
        }

        public void SetAsyncMode(bool enabled)
        {
            // Schedule it thread save via the Dispatcher
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                isRunningProgressBar.IsIndeterminate = enabled;
            }));
        }

 
    }
}

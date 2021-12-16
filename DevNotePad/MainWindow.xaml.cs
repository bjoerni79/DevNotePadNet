using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.ViewModel;
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
    public partial class MainWindow : Window, IMainViewUi
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetScrollbars(bool enable)
        {
            var scrollbarMode = ScrollBarVisibility.Auto;
            if (enable)
            {
                scrollbarMode = ScrollBarVisibility.Visible;
            }

            editor.HorizontalScrollBarVisibility = scrollbarMode;
            editor.VerticalScrollBarVisibility = scrollbarMode;
        }

        public void SetWordWrap(bool enable)
        {
            var wrapping = TextWrapping.NoWrap;
            if (enable)
            {
                wrapping = TextWrapping.Wrap;
            }

            editor.TextWrapping = wrapping;
        }

        public void ShowAbout()
        {
            var aboutDialog = new About();
            aboutDialog.Owner = this;
            aboutDialog.ShowDialog();
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

            var vm = DataContext as IMainViewModel;
            if (vm != null)
            {
                vm.Init(this);
                vm.ApplySettings();
            }
        }
    }
}

using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared;
using DevNotePad.Shared.Dialog;
using DevNotePad.Shared.Event;
using DevNotePad.UI;
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
    public partial class MainWindow : Window, IMainViewUi, IEventListener
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        #region IMainViewUI


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

            //editor.TextWrapping = wrapping;
        }


        public void ShowAbout()
        {
            //TODO: Move this to the dialog service
            var aboutDialog = new About();
            aboutDialog.Owner = this;
            aboutDialog.ShowInTaskbar = false;
            aboutDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            aboutDialog.ShowDialog();
        }

        public void SetFilename(string filename)
        {
            //TODO: Move the logic to the View Model
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var filenameDescriptor = filename;
                var maxLength = 50;
                if (filename.Length > maxLength)
                {
                    var length = filename.Length;
                    filenameDescriptor = "..." + filename.Substring(length - maxLength, maxLength);
                }

                var newTitle = string.Format("DevNotePad - {0}", filenameDescriptor);
                mainWindow.Title = newTitle;
            }));
        }

        public void CloseByViewModel()
        {
            Close();
        }

        public void ResetLayout()
        {

        }

        #endregion

        #region IEventListener

        public void OnTrigger(string eventId)
        {
            //None
        }

        public void OnTrigger(string eventId, object parameter)
        {
            // All actions in this method are thread sensitive! Use the dispatcher for UI changes only.

            if (eventId == Events.UpdateToolBarEvent)
            {
                var updateStatusBarParameter = parameter as UpdateStatusBarParameter;
                if (updateStatusBarParameter != null)
                {
                    Dispatcher.BeginInvoke(new Action(() => ApplyNotification(updateStatusBarParameter)));
                }
            }

            if (eventId == Events.UpdateAsyncStateEvent)
            {
                var updateAsyncState = parameter as UpdateAsyncProcessState;
                if (updateAsyncState != null)
                {
                    // Schedule it thread save via the Dispatcher
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // Render the progress bar depending on the state.
                        if (updateAsyncState.InProgress)
                        {
                            isRunningProgressBar.Visibility = Visibility.Visible;
                            
                        }
                        else
                        {
                            isRunningProgressBar.Visibility = Visibility.Hidden;
                        }

                        // Disable the textboxes while I/O operation is in progresss
                        editor.IsReadOnly = updateAsyncState.InProgress;
                    }));
                }
            }
        }

        private void ApplyNotification(UpdateStatusBarParameter e)
        {
            var styleDefault = "notificationDefault";
            var styleWarning = "notificationWarning";

            var message = e.Message;
            var isWarning = e.IsWarning;

            Style style;
            if (isWarning)
            {
                style = App.Current.FindResource(styleWarning) as Style;
            }
            else
            {
                style = App.Current.FindResource(styleDefault) as Style;
            }

            notificationLabel.Content = message;
            notificationLabel.Style = style;
        }

        #endregion

        #region Event Delegates

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var facade = FacadeFactory.Create();
            if (facade != null)
            {
                var eventController = facade.Get<EventController>(Bootstrap.EventControllerId);
                var updateToolbarEvent = eventController.GetEvent(Events.UpdateToolBarEvent);
                var asyncOperationEvent = eventController.GetEvent(Events.UpdateAsyncStateEvent);

                updateToolbarEvent.AddListener(this);
                asyncOperationEvent.AddListener(this);

                IDialogService dialogService = new DialogService(this);
                facade.AddUnique(dialogService,Bootstrap.DialogServiceId);

                IToolDialogService toolDialogService = new ToolDialogService(this);
                facade.AddUnique(toolDialogService,Bootstrap.ToolDialogServiceId);
            }

            var vm = GetViewModel();
            if (vm != null)
            {
                // vm.Init(this, new TextComponent2(editor), new TextComponent(scratchPad));
                vm.Init(this, new TextComponent2(editor));
                vm.ApplySettings();
            }

            if (StartUpCondition.FileName != null)
            {
                // Open the file
                vm.OpenExternalFile(StartUpCondition.FileName);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = GetViewModel();
            if (vm != null)
            {
                var needsSaving = vm.IsChanged();
                if (needsSaving)
                {
                    var facade = FacadeFactory.Create();
                    if (facade != null)
                    {
                        var dialogService = facade.Get<IDialogService>(Bootstrap.DialogServiceId);
                        if (dialogService != null)
                        {
                            //TODO: Ask if the user wants to save first
                            var doClose = dialogService.ShowConfirmationDialog("There are pending changes. Do you want to close?", "Close","Close Application");
                            e.Cancel = !doClose;
                        }
                    }
                }
            }
        }

        private void editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update the view model
            var vm = GetViewModel();
            if (vm != null)
            {
                var textChange = e.Changes.FirstOrDefault();
                if (textChange != null)
                {
                    vm.NotifyContentChanged(textChange.AddedLength, textChange.Offset, textChange.RemovedLength);
                }

                //// Old
                //var textChange = e.Changes.First();
                //vm.NotifyContentChanged(textChange.AddedLength, textChange.Offset, textChange.RemovedLength);
            }

        }

        #endregion

        private IMainViewModel? GetViewModel()
        {
            var vm = DataContext as IMainViewModel;
            return vm;
        }

        private void scratchPad_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update the view model
            var vm = GetViewModel();
            if (vm != null)
            {
                var textChange = e.Changes.First();
                vm.NotifyScratchPadContentChanged(textChange.AddedLength, textChange.Offset, textChange.RemovedLength);
            }
        }
    }
}

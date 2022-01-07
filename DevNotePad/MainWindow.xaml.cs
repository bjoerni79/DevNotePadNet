using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared;
using DevNotePad.Shared.Event;
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

        public void CleanUpScratchPad()
        {
            scratchPad.Text = String.Empty;
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
            //TODO: Move this to the dialog service
            var aboutDialog = new About();
            aboutDialog.Owner = this;
            aboutDialog.ShowInTaskbar = false;
            aboutDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            aboutDialog.ShowDialog();
        }

        public void SetText(string text)
        {
            SetText(text, false);
        }

        public void SetText(string text, bool selected)
        {
            editor.BeginChange();

            if (selected)
            {
                editor.SelectedText = text;
            }
            else
            {
                editor.Text = text;
            }

            editor.EndChange();
        }

        public string GetText(bool selected)
        {
            if (selected)
            {
                var selectedText = editor.SelectedText;
                return selectedText;
            }
            else
            {
                return editor.Text;
            }
        }

        public int GetCurrentPosition()
        {
            return editor.CaretIndex;
        }
        public void SetFilename(string filename)
        {
            //TODO: Move the logic to the View Model
            var filenameDescriptor = filename;
            var maxLength = 50;
            if (filename.Length > maxLength)
            {
                var length = filename.Length;
                filenameDescriptor = "..." + filename.Substring(length - maxLength, maxLength);
            }

            var newTitle = string.Format("DevNotePad - {0}", filenameDescriptor);
            mainWindow.Title = newTitle;
        }

        public bool IsTextSelected()
        {
            var selectedText = editor.SelectedText;
            return !string.IsNullOrWhiteSpace(selectedText);
        }

        public void AddToScratchPad(string text)
        {
            var content = new StringBuilder();
            content.Append(scratchPad.Text);
            content.AppendFormat("\n###\n{0}\n", text);

            scratchPad.Text = content.ToString();
        }

        public void FocusTree()
        {
            tabTreeView.Focus();
        }

        public void FocusScratchPad()
        {
            tabScratchPad.Focus();
        }

        public void CloseByViewModel()
        {
            Close();
        }

        public void SelectText(int startIndex, int length)
        {
            editor.Focus();
            editor.Select(startIndex, length);

            //var selectedText = editor.SelectedText;

        }

        public void PerformClipboardAction(ClipboardActionEnum action)
        {

            switch (action)
            {
                case ClipboardActionEnum.Copy:
                    editor.Copy();
                    break;
                case ClipboardActionEnum.Paste:
                    editor.Paste();
                    break;
                case ClipboardActionEnum.Cut:
                    editor.Cut();
                    break;
                case ClipboardActionEnum.SelectAll:
                    editor.SelectAll();
                    break;
            }
        }

        #endregion

        #region IEventListener

        public void OnTrigger(string eventId)
        {
            //None
        }

        public void OnTrigger(string eventId, object parameter)
        {
            if (eventId == Bootstrap.UpdateToolBarEvent)
            {
                var updateStatusBarParameter = parameter as UpdateStatusBarParameter;
                if (updateStatusBarParameter != null)
                {
                    ApplyNotification(updateStatusBarParameter);
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
                style = Resources[styleWarning] as Style;
            }
            else
            {
                style = Resources[styleDefault] as Style;
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
                var updateToolbarEvent = eventController.GetEvent(Bootstrap.UpdateToolBarEvent);
                updateToolbarEvent.AddListener(this);

                IDialogService dialogService = new DialogService(this);
                facade.AddUnique(dialogService,Bootstrap.DialogServiceId);
            }

            var vm = GetViewModel();
            if (vm != null)
            {
                vm.Init(this);
                vm.ApplySettings();
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
                var textChange = e.Changes.First();
                vm.NotifyContentChanged(textChange.AddedLength, textChange.Offset, textChange.RemovedLength);
            }

        }

        #endregion

        private IMainViewModel? GetViewModel()
        {
            var vm = DataContext as IMainViewModel;
            return vm;
        }

    }
}

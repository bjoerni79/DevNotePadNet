﻿using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared.Dialog;
using DevNotePad.Shared.Event;
using DevNotePad.UI;
using DevNotePad.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            DataContext = App.Current.BootStrap.Services.GetService<MainViewModel>();
        }

        #region IMainViewUI

        public void SetNotifier(bool enabled)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var visibleState = Visibility.Collapsed;

                if (enabled)
                {
                    visibleState = Visibility.Visible;
                }

                notifierPanel.Visibility = visibleState;
            }));
        }

        public void SetScrollbars(bool enable)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var scrollbarMode = ScrollBarVisibility.Auto;
                if (enable)
                {
                    scrollbarMode = ScrollBarVisibility.Visible;
                }

                editor.HorizontalScrollBarVisibility = scrollbarMode;
                editor.VerticalScrollBarVisibility = scrollbarMode;
            }));
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

        #region Event Delegates

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterUiServices();

            var vm = GetViewModel();
            if (StartUpCondition.FileName != null)
            {
                // Open the file
                vm.OpenExternalFile(StartUpCondition.FileName);
            }
        }

        private void RegisterUiServices()
        {
            var vm = GetViewModel();
            if (vm != null)
            {
                // vm.Init(this, new TextComponent2(editor), new TextComponent(scratchPad));
                vm.Init(this, new TextComponent2(editor));
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
                    // Ask the user if there are pending changes only if it is enabled.

                    var settings = ServiceHelper.GetSettings();
                    if (settings.IgnoreChanged)
                    {
                        var dialogService = App.Current.BootStrap.Services.GetService<IDialogService>();
                        if (dialogService != null)
                        {
                            var doClose = dialogService.ShowConfirmationDialog("There are pending changes. Do you want to close?", "Close", "Close Application");
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

using CommunityToolkit.Mvvm.Messaging;
using DevNotePad.Service;
using DevNotePad.Shared;
using DevNotePad.Shared.Event;
using DevNotePad.Shared.Message;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace DevNotePad.MVVM
{
    /// <summary>
    /// Helper class for accessing the service in the MVVM IoC container
    /// </summary>
    internal static class ServiceHelper
    {
        /// <summary>
        /// Gets the IDialog service
        /// </summary>
        /// <returns>The dialog service</returns>
        /// <exception cref="Exception">An exception is thrown if the service is not available</exception>
        internal static IDialogService GetDialogService()
        {
            var dialogService = App.Current.BootStrap.Services.GetService<IDialogService>();

            if (dialogService == null)
            {
                throw new Exception("Cannot access DialogService");
            }

            return dialogService;

        }

        internal static IToolDialogService GetToolDialogService()
        {
            var toolDialogService = App.Current.BootStrap.Services.GetService<IToolDialogService>();
            if (toolDialogService == null)
            {
                throw new Exception("Cannot access DialogService");
            }

            return toolDialogService;
        }



        /// <summary>
        /// Gets the I/O service
        /// </summary>
        /// <returns>the I/O service</returns>
        /// <exception cref="Exception">An exception is thrown if the service is not available</exception>
        internal static IIoService GetIoService()
        {
            var ioService = App.Current.BootStrap.Services.GetService<IIoService>();
            if (ioService == null)
            {
                throw new Exception("Cannot access I/O Service");
            }

            return ioService;
        }

        /// <summary>
        /// Shows the error dialog via the Dialog Service
        /// </summary>
        /// <param name="exception">the exception</param>
        /// <param name="component">the component</param>
        /// <exception cref="Exception">An exception is thrown if the dialog service is not available</exception>
        internal static void ShowError(Exception exception, string component)
        {
            var dialogService = GetDialogService();
            dialogService.ShowErrorDialog(exception, component);
        }


        internal static void ShowWarning(string warning, string component)
        {
            var dialogService = GetDialogService();
            dialogService.ShowWarningDialog(warning, component);
        }

        internal static Settings GetSettings()
        {
            var settingsService = App.Current.BootStrap.Services.GetService<ISettingsService>();
            var settings = settingsService.GetSettings();

            return settings;
        }

        internal static void TriggerToolbarNotification (UpdateStatusBarParameter parameter)
        {
            TriggerNotiferViewVisible(true);
            WeakReferenceMessenger.Default.Send(new UpdateStatusBarParameterMessage(parameter));
        }

        internal static void TriggerStartStopAsnyOperation(UpdateAsyncProcessState state)
        {
            TriggerNotiferViewVisible(true);
            WeakReferenceMessenger.Default.Send(new UpdateAsyncProcessStateMessage(state.InProgress));
        }

        internal static void TriggerFileUpdate(EditorState state)
        {
            TriggerNotiferViewVisible(true);
            WeakReferenceMessenger.Default.Send(new UpdateFileStatusMessage(state));
        }

        internal static void TriggerNotiferViewVisible(bool isVisible)
        {
            WeakReferenceMessenger.Default.Send(new NotfierVisibleMessage(isVisible));
        }
    }
}

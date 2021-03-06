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
            //var facade = GetFacade();
            //var dialogService = facade.Get<IDialogService>(Bootstrap.DialogServiceId);
            //if (dialogService == null)
            //{
            //    throw new Exception("Cannot access DialogService");
            //}

            //return dialogService;
            var dialogService = App.Current.BootStrap.Services.GetService<IDialogService>();

            if (dialogService == null)
            {
                throw new Exception("Cannot access DialogService");
            }

            return dialogService;

        }

        internal static IToolDialogService GetToolDialogService()
        {
            //var facade = GetFacade();
            //var dialogService = facade.Get<IToolDialogService>(Bootstrap.ToolDialogServiceId);

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
            //var facade = GetFacade();
            //var ioService = facade.Get<IIoService>(Bootstrap.IoServiceId);

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
            //Settings? settings = null;

            //var facade = FacadeFactory.Create();
            //if (facade != null)
            //{
            //    settings = facade.Get<Settings>(Bootstrap.SettingsId);
            //}

            //if (settings == null)
            //{
            //    throw new ArgumentNullException("settings");
            //}

            //return settings;

            var settingsService = App.Current.BootStrap.Services.GetService<ISettingsService>();
            var settings = settingsService.GetSettings();

            return settings;
        }

        internal static void TriggerToolbarNotification (UpdateStatusBarParameter parameter)
        {
            WeakReferenceMessenger.Default.Send(new UpdateStatusBarParameterMessage(parameter));
        }

        internal static void TriggerStartStopAsnyOperation(UpdateAsyncProcessState state)
        {
            WeakReferenceMessenger.Default.Send(new UpdateAsyncProcessStateMessage(state.InProgress));
        }

        internal static void TriggerFileUpdate(EditorState state)
        {
            WeakReferenceMessenger.Default.Send(new UpdateFileStatusMessage(state));
        }
    }
}

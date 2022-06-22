using CommunityToolkit.Mvvm.Messaging;
using DevNotePad.Service;
using DevNotePad.Shared;
using DevNotePad.Shared.Event;
using DevNotePad.Shared.Message;
using Generic.MVVM.Event;
using Generic.MVVM.IOC;
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
            var facade = GetFacade();
            var dialogService = facade.Get<IDialogService>(Bootstrap.DialogServiceId);
            if (dialogService == null)
            {
                throw new Exception("Cannot access DialogService");
            }

            return dialogService;
        }

        internal static IToolDialogService GetToolDialogService()
        {
            var facade = GetFacade();
            var dialogService = facade.Get<IToolDialogService>(Bootstrap.ToolDialogServiceId);
            if (dialogService == null)
            {
                throw new Exception("Cannot access DialogService");
            }

            return dialogService;
        }

        internal static IEvent GetEvent(string eventId)
        {
            var facade = GetFacade();
            var eventController = facade.Get<EventController>(Bootstrap.EventControllerId);

            return eventController.GetEvent(eventId);
        }

        /// <summary>
        /// Gets the I/O service
        /// </summary>
        /// <returns>the I/O service</returns>
        /// <exception cref="Exception">An exception is thrown if the service is not available</exception>
        internal static IIoService GetIoService()
        {
            var facade = GetFacade();
            var ioService = facade.Get<IIoService>(Bootstrap.IoServiceId);
            if (ioService == null)
            {
                throw new Exception("Cannot access I/O Service");
            }

            return ioService;
        }

        /// <summary>
        /// Gets the facade of the MVVM IoC. It provides a generic access to the container
        /// </summary>
        /// <returns>the container facade</returns>
        /// <exception cref="Exception">An exception is thrown if the facade is not available</exception>
        internal static ContainerFacade GetFacade()
        {
            var facade = FacadeFactory.Create();
            if (facade == null)
            {
                throw new Exception("Cannot access MVVM facade");
            }

            return FacadeFactory.Create();
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

        /// <summary>
        /// Shows the error dialog via the Dialog Service
        /// </summary>
        /// <param name="exception">the exception</param>
        /// <param name="component">the component</param>
        /// <exception cref="Exception">An exception is thrown if the dialog service is not available</exception>
        internal static void ShowError(Exception exception, string component, Window owner)
        {
            var dialogService = GetDialogService();
            dialogService.ShowErrorDialog(exception, component, owner);
        }

        internal static void ShowWarning(string warning, string component, Window owner)
        {
            var dialogService = GetDialogService();
            dialogService.ShowWarningDialog(warning, component, owner);
        }

        internal static Settings GetSettings()
        {
            Settings? settings = null;

            var facade = FacadeFactory.Create();
            if (facade != null)
            {
                settings = facade.Get<Settings>(Bootstrap.SettingsId);
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

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

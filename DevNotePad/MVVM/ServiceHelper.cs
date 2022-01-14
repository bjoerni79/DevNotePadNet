using DevNotePad.Service;
using DevNotePad.Shared.Event;
using Generic.MVVM.Event;
using Generic.MVVM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Triggers the UpdateToolbar event. 
        /// </summary>
        /// <param name="parameter">the parameter for the event</param>
        /// <exception cref="Exception">An exception is thrown if the facade  is not available</exception>
        internal static void TriggerToolbarNotification(UpdateStatusBarParameter parameter)
        {
            var facade = GetFacade();
            var eventController = facade.Get<EventController>();

            var eventInstance = eventController.GetEvent(Bootstrap.UpdateToolBarEvent);
            if (eventInstance != null)
            {
                eventInstance.Trigger(parameter);
            }
        }
    }
}

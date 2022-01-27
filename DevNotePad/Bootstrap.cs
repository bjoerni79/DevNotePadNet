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

namespace DevNotePad
{
    public class Bootstrap
    {
        //TODO: Remove these values from the bootstrap class
        public const string BootstrapId = "bootstrap";
        public const string EventControllerId = "eventcontrollerid";
        public const string DialogServiceId = "dialogserviceid";
        public const string IoServiceId = "ioserviceid";
        public const string SettingsId = "settingsid";

        public const string UpdateToolBarEvent = "updatetoolbarevent";
        public const string UpdateAsyncStateEvent = "updateasyncstateevent";
        public const string UpdateFileStateEvent = "updatefilestateevent";

        public const string ViewModelFindDialog = "viewmodelfinddialog";
        public const string ViewModelReplaceDialog = "viewmodelreplacedialog";
        public const string ViewModelBase64Dialog = "viewModelbase64dialog";

        public Bootstrap()
        {
            FacadeFactory.InitFactory();

            Main = new MainViewModel();
        }

        public void Init()
        {
            InitComponents();
            LoadSettings();
        }

        private void InitComponents()
        {
            //Init the eventController and services
            var eventController = new EventController();
            IIoService ioService = new IoService();

            // Register Events
            RegisterEvents(eventController);

            // Add it to the IoC container 
            var facade = FacadeFactory.Create();
            facade.AddUnique(eventController, EventControllerId);
            facade.AddUnique(ioService, IoServiceId);
        }

        private void RegisterEvents(EventController eventController)
        {
            var updateStatusEvent = new Generic.MVVM.Event.Event(UpdateToolBarEvent);
            var asyncOperationEvent = new Generic.MVVM.Event.Event(UpdateAsyncStateEvent);
            var updateFileStateEvent = new Generic.MVVM.Event.Event(UpdateFileStateEvent);

            eventController.Add(updateStatusEvent);
            eventController.Add(asyncOperationEvent);
            eventController.Add(updateFileStateEvent);
        }

        private void LoadSettings()
        {
            //TODO: Load the settings, if found..
            var settings = new Settings();

            // Store it in the container
            var facade = FacadeFactory.Create();
            facade.AddUnique(settings, SettingsId);
        }

        public MainViewModel Main { get; set; }
    }
}

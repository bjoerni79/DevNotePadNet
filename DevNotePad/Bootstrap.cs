using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared;
using DevNotePad.ViewModel;
using Generic.MVVM.Event;

namespace DevNotePad
{
    public class Bootstrap
    {
        // See ViewModelInstances and Events for other
        public const string BootstrapId = "bootstrap";
        public const string EventControllerId = "eventcontrollerid";
        public const string DialogServiceId = "dialogserviceid";
        public const string ToolDialogServiceId = "tooldialogserviceid";
        public const string IoServiceId = "ioserviceid";
        public const string SettingsId = "settingsid";

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
            var updateStatusEvent = new Generic.MVVM.Event.Event(Events.UpdateToolBarEvent);
            var asyncOperationEvent = new Generic.MVVM.Event.Event(Events.UpdateAsyncStateEvent);
            var updateFileStateEvent = new Generic.MVVM.Event.Event(Events.UpdateFileStateEvent);
            var updateTreeEvent = new Generic.MVVM.Event.Event(Events.UpdateTreeEvent);

            eventController.Add(updateStatusEvent);
            eventController.Add(asyncOperationEvent);
            eventController.Add(updateFileStateEvent);
            eventController.Add(updateTreeEvent);
        }

        private void LoadSettings()
        {
            //TODO: Load the settings, if found..
            var settings = Settings.GetDefault();

            // Store it in the container
            var facade = FacadeFactory.Create();
            facade.AddUnique(settings, SettingsId);
        }

        public MainViewModel Main { get; set; }
    }
}

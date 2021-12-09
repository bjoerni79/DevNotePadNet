using DevNotePad.MVVM;
using DevNotePad.Service;
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
        public const string BootstrapId = "bootstrap";
        public const string EventControllerId = "eventcontrollerid";
        public const string DialogServiceId = "dialogserviceid";
        public const string IoServiceId = "ioserviceid";

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

            var facade = FacadeFactory.Create();
            facade.AddUnique(eventController, EventControllerId);
            facade.AddUnique(ioService, IoServiceId);
        }

        private void LoadSettings()
        {
            //TODO: See class Settings
        }

        public MainViewModel Main { get; set; }
    }
}

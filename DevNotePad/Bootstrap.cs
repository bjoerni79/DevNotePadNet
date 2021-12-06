using DevNotePad.MVVM;
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

        public Bootstrap()
        {
            FacadeFactory.InitFactory();

            Main = new MainViewModel();
        }

        public void Init()
        {
            //Init the EventController
            var eventController = new EventController();

            var facade = FacadeFactory.Create();
            facade.AddUnique<EventController>(eventController, EventControllerId);
        }

        public MainViewModel Main { get; set; }
    }
}

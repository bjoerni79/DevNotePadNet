using DevNotePad.Features.Shared;
using DevNotePad.MVVM;
using Generic.MVVM;
using Generic.MVVM.Event;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevNotePad.ViewModel
{
    public class TreeViewModel : MainViewUiViewModel, IEventListener
    {

        public TreeViewModel()
        {
            var facade = FacadeFactory.Create();
            var eventController = facade.Get<EventController>(Bootstrap.EventControllerId);

            var updateTreeEvent = eventController.GetEvent(Bootstrap.UpdateTreeEvent);
            updateTreeEvent.AddListener(this);

            Close = new DefaultCommand(() => dialog.CloseDialog(true));
        }

        public IRefreshCommand? Close { get; set; }

        public ObservableCollection<ItemNode>? Nodes { get; set; }

        public void OnTrigger(string eventId)
        {
            // None
        }

        public void OnTrigger(string eventId, object parameter)
        {
            if (eventId == Bootstrap.UpdateTreeEvent)
            {
                var itemNodes = parameter as IEnumerable<ItemNode>;
                if (itemNodes != null)
                {
                    Nodes = new ObservableCollection<ItemNode>(itemNodes);
                }
                else
                {
                    Nodes = null;
                }

                RaisePropertyChange("Nodes");
            }
        }
    }
}

using CommunityToolkit.Mvvm.Input;
using DevNotePad.Features.Shared;
using DevNotePad.MVVM;
using DevNotePad.Shared.Event;
using Generic.MVVM.Event;
using System.Collections.ObjectModel;

namespace DevNotePad.ViewModel
{
    public class TreeViewModel : MainViewUiViewModel, IEventListener
    {

        public TreeViewModel()
        {
            var facade = FacadeFactory.Create();
            var eventController = facade.Get<EventController>(Bootstrap.EventControllerId);

            var updateTreeEvent = eventController.GetEvent(Events.UpdateTreeEvent);
            updateTreeEvent.AddListener(this);

            Close = new RelayCommand(() => dialog.CloseDialog(true));
        }

        public RelayCommand? Close { get; set; }

        public ObservableCollection<ItemNode>? Nodes { get; set; }

        public void OnTrigger(string eventId)
        {
            // None
        }

        public void OnTrigger(string eventId, object parameter)
        {
            if (eventId == Events.UpdateTreeEvent)
            {
                var updateParameter = parameter as UpdateTree;
                if (updateParameter != null)
                {
                    Nodes = new ObservableCollection<ItemNode>(updateParameter.ItemNodes);
                }
                else
                {
                    Nodes = null;
                }

                OnPropertyChanged("Nodes");
            }
        }
    }
}

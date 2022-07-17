using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DevNotePad.Features.Shared;
using DevNotePad.MVVM;
using DevNotePad.Shared.Event;
using DevNotePad.Shared.Message;
using System.Collections.ObjectModel;

namespace DevNotePad.ViewModel
{
    public class TreeViewModel : MainViewUiViewModel
    {
        // https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/messenger

        public TreeViewModel()
        {
            //WeakReferenceMessenger.Default.Register<UpdateTreeMessage>(this);

            Close = new RelayCommand(() => dialog.CloseDialog(true));

            IsActive = true;
        }

        protected override void OnActivated()
        {
            Messenger.Register<TreeViewModel, UpdateTreeMessage>(this, (vm, m) => Receive(m));
        }

        public RelayCommand? Close { get; set; }

        public ObservableCollection<ItemNode>? Nodes { get; set; }

        public void Receive(UpdateTreeMessage message)
        {
            var nodes = message.Value;
            if (nodes != null)
            {
                Nodes = new ObservableCollection<ItemNode>(nodes);
            }
            else
            {
                Nodes = null;
            }

            OnPropertyChanged("Nodes");
        }

    }
}

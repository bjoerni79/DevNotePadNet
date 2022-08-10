using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DevNotePad.MVVM;
using DevNotePad.Shared.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class NotifierViewModel : ObservableRecipient
    {
        private INotifierView? view;

        public NotifierViewModel()
        {
            // Required for custom message handling. See OnActivated.
            IsActive = true;

            Close = new RelayCommand(OnClose);
        }

        public void RegisterView(INotifierView view)
        {
            this.view = view;
        }

        protected override void OnActivated()
        {
            // Register the two events. The interface method can only deal with one.
            Messenger.Register<NotifierViewModel, UpdateAsyncProcessStateMessage>(this, (r, m) => SetAsyncMode(m.Value));

            // Register the UpdateStatusBarParameterMessage
            Messenger.Register<NotifierViewModel, UpdateStatusBarParameterMessage>(this, (r, m) => UpdateMessage(m));
        }

        private void UpdateMessage(UpdateStatusBarParameterMessage message)
        {
            var parameter = message.Value;
            Message = parameter.Message;
            IsWarning = parameter.IsWarning;

            OnPropertyChanged("Message");
            OnPropertyChanged("IsWarning");
        }

        private void SetAsyncMode (bool isAsync)
        {
            if (view != null)
            {
                view.SetAsyncMode(isAsync);
            }
        }

        private void OnClose()
        {
            // Sent the event, that the Notifier shall be closed. The MainViewModel gets it and takes care of it
            ServiceHelper.TriggerNotiferViewVisible(false);
        }

        public IRelayCommand Close { get; private set; }

        public string? Message { get; private set; }

        public bool IsWarning { get; private set; }
    }
}

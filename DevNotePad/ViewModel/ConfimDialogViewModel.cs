using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNotePad.Shared.Dialog;

namespace DevNotePad.ViewModel
{
    public class ConfimDialogViewModel : ObservableObject
    {
        public ConfimDialogViewModel()
        {
            Confirm = new RelayCommand(OnConfirm);
            Cancel = new RelayCommand(OnCancel);

            Question = "Question";
            DialogTitle = "Confirm Dialog";
            ConfirmButtonText = "OK";
        }

        private IDialog? dialog;

        public string Question { get; set; }

        public string DialogTitle { get; set; }

        public string ConfirmButtonText { get; set; }

        public RelayCommand Confirm { get; set; }

        public RelayCommand Cancel { get; set; }

        private void OnConfirm()
        {
            dialog!.CloseDialog(true);
        }

        private void OnCancel()
        {
            dialog!.CloseDialog(false);
        }

        public void Init(string question, string title, string okButtonText, IDialog dialog)
        {
            this.dialog = dialog;

            if (!string.IsNullOrEmpty(question))
            {
                Question = question;
                OnPropertyChanged("Question");
            }

            if (!string.IsNullOrEmpty(okButtonText))
            {
                ConfirmButtonText = okButtonText;
                OnPropertyChanged("ConfirmButtonText");
            }


            if (!string.IsNullOrEmpty(title))
            {
                DialogTitle = title;
                OnPropertyChanged("DialogTitle");
            }
        }
    }
}

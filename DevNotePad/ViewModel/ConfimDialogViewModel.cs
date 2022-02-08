using DevNotePad.Shared;
using DevNotePad.Shared.Dialog;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevNotePad.ViewModel
{
    public class ConfimDialogViewModel : AbstractViewModel
    {
        public ConfimDialogViewModel()
        {
            Confirm = new DefaultCommand(OnConfirm);
            Cancel = new DefaultCommand(OnCancel);

            Question = "Question";
            DialogTitle = "Confirm Dialog";
            ConfirmButtonText = "OK";
        }

        private IDialog? dialog;

        public string Question { get; set; }

        public string DialogTitle { get; set; }

        public string ConfirmButtonText { get; set; }

        public ICommand Confirm { get; set; }

        public ICommand Cancel { get; set; }

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
                RaisePropertyChange("Question");
            }

            if (!string.IsNullOrEmpty(okButtonText))
            {
                ConfirmButtonText = okButtonText;
                RaisePropertyChange("ConfirmButtonText");
            }


            if (!string.IsNullOrEmpty(title))
            {
                DialogTitle = title;
                RaisePropertyChange("DialogTitle");
            }
        }
    }
}

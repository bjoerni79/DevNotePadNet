using DevNotePad.Shared;
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
        }

        private IDialog? dialog;

        public string Question { get; set; }

        public string DialogTitle { get; set; }

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

        public void Init(string question, string title, IDialog dialog)
        {
            Question = question;
            RaisePropertyChange("Question");

            this.dialog = dialog;

            if (!string.IsNullOrEmpty(title))
            {
                DialogTitle = title;
                RaisePropertyChange("DialogTitle");
            }
        }
    }
}

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
    public class OkDialogViewModel : AbstractViewModel
    {
        public OkDialogViewModel()
        {
            DialogTitle = "Message Box";
            Component = "Unknown";
            Message = "Test Message";

            Confirm = new DefaultCommand(OnConfirm);
        }

        private IDialog? dialog;

        public void Init(string message, string component, string title, IDialog dialog)
        {
            Message = message;
            Component = component;
            DialogTitle = title;
            this.dialog = dialog;
        }

        public string DialogTitle { get; set; }

        public string Component { get; set; }

        public string Message { get; set; }

        public bool ShowDetals { get; set; }

        public string? Details { get; set; }

        // OK Button
        public ICommand Confirm { get; set; }

        private void OnConfirm()
        {
            dialog!.CloseDialog(true);
        }
    }

}

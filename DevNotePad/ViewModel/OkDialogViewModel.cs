using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNotePad.Shared;
using DevNotePad.Shared.Dialog;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DevNotePad.ViewModel
{
    public class OkDialogViewModel : ObservableObject
    {
        public OkDialogViewModel()
        {
            DialogTitle = "Message Box";
            Component = "Unknown";
            Message = "Test Message";

            Confirm = new RelayCommand(OnConfirm);
            Copy = new RelayCommand(OnCopy);
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
        public RelayCommand Confirm { get; set; }

        public RelayCommand Copy { get; set; }

        private void OnConfirm()
        {
            dialog!.CloseDialog(true);
        }

        private void OnCopy()
        {
            var textBuilder = new StringBuilder();
            textBuilder.AppendFormat("Dialog Title = {0}\n", DialogTitle);
            textBuilder.AppendFormat("Component = {0}\n", Component);
            textBuilder.AppendFormat("Message = {0}\n", Message);
            if (Details != null)
            {
                textBuilder.AppendFormat("Details = {0}", Details);
            }

            Clipboard.SetText(textBuilder.ToString());
        }
    }

}

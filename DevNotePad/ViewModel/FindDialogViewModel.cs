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
    public class FindDialogViewModel : AbstractViewModel
    {
        private IMainViewUi ui;
        private IDialog dialog;

        public FindDialogViewModel(IMainViewUi mainViewUi, IDialog dialogUi)
        {
            ui = mainViewUi;
            dialog = dialogUi;

            FindNext = new DefaultCommand(OnFindNext);
            Cancel = new DefaultCommand(OnCancel);
        }


        public bool IgnoreLetterType { get; set; }

        public string? SearchPattern { get; set; }

        public ICommand FindNext { get; set; }

        public ICommand Cancel { get; set; }

        private void OnFindNext()
        {
            // Write some clever code.
            var content = ui.GetText(false);
            bool ignoreCapitalLetter = IgnoreLetterType;

            //TODO: 

            // Trigger test
            TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Find Next clicked", false));
        }

        

        private void OnCancel()
        {
            dialog.CloseDialog(true);
        }

        /// <summary>
        /// Specifies the result of the search operation
        /// </summary>
        /// <param name="Successful">true if any content has been found</param>
        /// <param name="start">the start index</param>
        /// <param name="length">the length</param>
        private record SearchResultValue(bool Successful, int start, int length);
    }
}

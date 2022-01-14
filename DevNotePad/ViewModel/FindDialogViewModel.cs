using DevNotePad.MVVM;
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
    public class FindDialogViewModel : MainViewUiViewModel
    {
        private int startIndex;
        private string? searchPattern;

        private SearchEngine searchEngine;

        public FindDialogViewModel()
        {
            Find = new DefaultCommand(OnFind);
            FindNext = new DefaultCommand(OnFindNext,() => startIndex >= 0);
            Cancel = new DefaultCommand(OnCancel);

            startIndex = -1;
            searchEngine = new SearchEngine();
        }

        public bool IgnoreLetterType { get; set; }

        public bool StartFromCurrentPosition { get; set; }

        public string? SearchPattern
        {
            get
            {
                return searchPattern;
            }
            set
            {
                searchPattern = value;
            }
        }

        public IRefreshCommand Find { get; set; }

        public IRefreshCommand FindNext { get; set; }

        public IRefreshCommand Cancel { get; set; }

        private void OnFind()
        {
            if (StartFromCurrentPosition)
            {
                startIndex = ui.GetCurrentPosition();
            }
            else
            {
                startIndex = 0;
            }

            searchEngine.SearchPattern = SearchPattern;
            searchEngine.IgnoreLetterType = IgnoreLetterType;
            searchEngine.StartIndex = startIndex;

            FindNext.Refresh();
            OnFindNext();
        }

        private void OnFindNext()
        {
            var content = ui.GetText(false);

            var result = searchEngine.RunSearch(content);
            if (result.Successful)
            {
                ui.SelectText(result.StartIndex, result.Length);
                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Found", false));
            }
            else
            {
                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Not found", true));
            }
        }

        private void OnCancel()
        {
            dialog.CloseDialog(true);
        }


    }
}

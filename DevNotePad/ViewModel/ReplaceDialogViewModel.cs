using DevNotePad.Shared;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class ReplaceDialogViewModel : MainViewUiViewModel
    {
        private int startIndex;
        private bool searchState;

        private SearchEngine searchEngine;

        public ReplaceDialogViewModel()
        {
            startIndex = -1;
            searchEngine = new SearchEngine();

            Find = new DefaultCommand(OnFind);
            FindNext = new DefaultCommand(OnFindNext, () => startIndex >= 0);
            Replace = new DefaultCommand(OnReplace, () => searchState);
            ReplaceAll = new DefaultCommand(OnReplaceAll);
            Cancel = new DefaultCommand(OnCancel);
        }

        public string? SearchFor { get; set; }

        public string? ReplaceWith { get; set; }

        public bool IgnoreLetterType { get; set; }

        public bool StartFromCurrentPosition { get; set; }

        public IRefreshCommand Find { get; set; }

        public IRefreshCommand FindNext { get; set; }

        public IRefreshCommand Replace { get; set; }

        public IRefreshCommand ReplaceAll { get; set; }

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

            searchEngine.SearchPattern = SearchFor;
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
                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Found", false));
            }
            else
            {
                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Not found", true));
            }

            // Enable the replace feature if possible
            searchState = result.Successful;
            Replace.Refresh();
        }

        private void OnReplace()
        {
            if (string.IsNullOrEmpty(ReplaceWith))
            {
                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Replace string is empty", true));
            }
            else
            {
                var isSelectionAvailable = ui.IsTextSelected();
                if (isSelectionAvailable)
                {
                    ui.SetText(ReplaceWith, true);
                }

                string notifier = "Search Pattern is replaced";
                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter(notifier, false));
            }

        }

        private void OnReplaceAll()
        {
            if (string.IsNullOrEmpty(ReplaceWith))
            {
                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Replace string is empty", true));
            }
            else
            {
                int replaceCount = 0;
                var content = ui.GetText(false);
                startIndex = 0;

                searchEngine.SearchPattern = SearchFor;
                searchEngine.StartIndex = startIndex;
                searchEngine.IgnoreLetterType = IgnoreLetterType;

                var result = searchEngine.RunSearch(content);
                while (result.Successful)
                {
                    ui.SelectText(result.StartIndex, result.Length);
                    ui.SetText(ReplaceWith, true);

                    // Increase the counter and update the search text
                    content = ui.GetText(false);
                    replaceCount++;

                    result = searchEngine.RunSearch(content);
                }

                string notifier = "Unknown";
                bool isWarning = true;
                if (replaceCount == 0)
                {
                    notifier = "None replace action performed";
                    isWarning = true;
                }
                if (replaceCount == 1)
                {
                    isWarning = false;
                    notifier = "One replace action performed";
                }
                if (replaceCount > 1)
                {
                    isWarning = false;
                    notifier = String.Format("Replaced action performed {0} times", replaceCount);
                }

                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter(notifier, isWarning));
            }




        }

        private void OnCancel()
        {
            dialog.CloseDialog(true);
        }
    }
}

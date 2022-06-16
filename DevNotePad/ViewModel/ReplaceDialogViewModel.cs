using DevNotePad.MVVM;
using DevNotePad.Shared;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DevNotePad.ViewModel
{
    public class ReplaceDialogViewModel : MainViewUiViewModel
    {
        private TextPointer? startIndex;
        private bool searchState;

        private SearchEngine searchEngine;

        public ReplaceDialogViewModel()
        {
            startIndex = null;
            searchEngine = new SearchEngine();

            Find = new DefaultCommand(OnFind);
            FindNext = new DefaultCommand(OnFindNext, () => startIndex != null);
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
                startIndex = textComponent.GetCurrentPosition();
            }
            else
            {
                startIndex = null;
            }

            searchEngine.SearchPattern = SearchFor;
            searchEngine.IgnoreLetterType = IgnoreLetterType;
            searchEngine.StartPosition = startIndex;

            FindNext.Refresh();
            InternalFind(false);
        }

        private void OnFindNext()
        {
            InternalFind(true);
        }

        private void OnReplace()
        {
            if (string.IsNullOrEmpty(ReplaceWith))
            {
                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Replace string is empty", true));
            }
            else
            {
                var isSelectionAvailable = textComponent.IsTextSelected();
                if (isSelectionAvailable)
                {
                    textComponent.SetText(ReplaceWith, true);
                }

                string notifier = "Search Pattern is replaced";
                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter(notifier, false));
            }
        }

        private void OnReplaceAll()
        {
            if (string.IsNullOrEmpty(ReplaceWith))
            {
                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Replace string is empty", true));
            }
            else
            {
                int replaceCount = 0;
                startIndex = null;

                searchEngine.SearchPattern = SearchFor;
                searchEngine.StartPosition = startIndex;
                searchEngine.IgnoreLetterType = IgnoreLetterType;

                //var result = searchEngine.RunSearch(content,true);
                var document = textComponent.GetDocument();
                var result = searchEngine.RunSearch(document,true);
                while (result.Successful)
                {
                    textComponent.SelectText(result.Selection);
                    textComponent.SetText(ReplaceWith, true);

                    // Increase the counter and update the search text
                    document = textComponent.GetDocument();
                    replaceCount++;

                    result = searchEngine.RunSearch(document,true);
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

                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter(notifier, isWarning));
            }
        }

        private void OnCancel()
        {
            dialog.CloseDialog(true);
        }

        private void InternalFind(bool findNext)
        {
            var currentDocument = textComponent.GetDocument();
            var result = searchEngine.RunSearch(currentDocument, findNext);
            if (result.Successful)
            {
                textComponent.SelectText(result.Selection);
                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Found", false));
            }
            else
            {
                startIndex = null;
                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Not found", true));
            }

            searchState = result.Successful;

            Replace.Refresh();
            FindNext.Refresh();
        }
    }
}

using CommunityToolkit.Mvvm.Input;
using DevNotePad.MVVM;
using DevNotePad.Shared;
using System.Windows.Documents;


namespace DevNotePad.ViewModel
{
    public class FindDialogViewModel : MainViewUiViewModel
    {
        private TextPointer? startIndex;
        private string? searchPattern;

        private SearchEngine searchEngine;

        public FindDialogViewModel()
        {
            Find = new RelayCommand(OnFind);
            FindNext = new RelayCommand(OnFindNext, () => startIndex != null);
            Cancel = new RelayCommand(OnCancel);

            startIndex = null;
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

        public RelayCommand Find { get; set; }

        public RelayCommand FindNext { get; set; }

        public RelayCommand Cancel { get; set; }

        private void OnFind()
        {
            // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/change-selection-in-a-richtextbox-programmatically?view=netframeworkdesktop-4.8

            if (StartFromCurrentPosition)
            {
                startIndex = textComponent.GetCurrentPosition();
            }
            else
            {
                startIndex = null;
            }

            searchEngine.SearchPattern = SearchPattern;
            searchEngine.IgnoreLetterType = IgnoreLetterType;
            searchEngine.StartPosition = startIndex;

            FindNext.NotifyCanExecuteChanged();
            InternalFind(false);
        }

        private void OnFindNext()
        {
            InternalFind(true);
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
                startIndex = result.Selection.End;

                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Found", false));
            }
            else
            {
                startIndex = null;
                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Not found", true));
            }

            FindNext.NotifyCanExecuteChanged();
        }
    }
}

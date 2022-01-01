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

        private int startIndex;
        private string? searchPattern;

        public FindDialogViewModel(IMainViewUi mainViewUi, IDialog dialogUi)
        {
            ui = mainViewUi;
            dialog = dialogUi;

            FindNext = new DefaultCommand(OnFindNext,()=>!string.IsNullOrEmpty(SearchPattern));
            Cancel = new DefaultCommand(OnCancel);

            startIndex = 0;
        }


        public bool IgnoreLetterType { get; set; }

        public string? SearchPattern
        {
            get
            {
                return searchPattern;
            }
            set
            {
                searchPattern = value;
                FindNext.Refresh();
            }
        }

        public IRefreshCommand FindNext { get; set; }

        public IRefreshCommand Cancel { get; set; }

        private void OnFindNext()
        {
            var content = ui.GetText(false);

            var result = RunSearch(content);
            if (result.Successful)
            {
                ui.SelectText(result.StartIndex, result.Length);
                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Found", false));
            }
            else
            {
                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Not found", true));
            }
        }

        private SearchResultValue RunSearch(string text)
        {
            //TODO: Start position?

            // https://docs.microsoft.com/en-us/dotnet/csharp/how-to/search-strings

            //
            //  Check if the pattern can be found
            //
            string content;
            int startIndex = 0;
            int length = 0;
            StringComparison comparison;

            if (startIndex > 0)
            {
                content = text.Substring(startIndex);
            }
            else
            { 
                content = text;
            }

            if (IgnoreLetterType)
            {
                comparison = StringComparison.CurrentCultureIgnoreCase;
            }
            else
            {
                comparison = StringComparison.CurrentCulture;
            }

            var found = content.Contains(SearchPattern, comparison);

            if (found)
            {
                startIndex = content.IndexOf(SearchPattern, comparison);
                length = SearchPattern.Length;
            }

            return new SearchResultValue(found,startIndex,length);
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
        private record struct SearchResultValue(bool Successful, int StartIndex, int Length);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DevNotePad.Shared
{
    internal class SearchEngine
    {
        internal string? SearchPattern { get; set; }

        internal bool IgnoreLetterType { get; set; }

        internal TextPointer? StartPosition { get; set; }

        internal SearchEngine()
        {
        }

        internal SearchResultValue RunSearch(FlowDocument flowDocument)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/how-to/search-strings

            StringComparison comparison;
            if (IgnoreLetterType)
            {
                comparison = StringComparison.CurrentCultureIgnoreCase;
            }
            else
            {
                comparison = StringComparison.CurrentCulture;
            }

            if (flowDocument == null || string.IsNullOrEmpty(SearchPattern))
            {
                return new SearchResultValue(false, null);
            }

            // Set the start index to the offset 
            if (StartPosition != null)
            {

            }

            SearchResultValue searchResult = new SearchResultValue(false, null); ;

            return searchResult;
        }

        internal SearchResultValue RunSearchOld(FlowDocument flowDocument)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/how-to/search-strings

            StringComparison comparison;
            if (IgnoreLetterType)
            {
                comparison = StringComparison.CurrentCultureIgnoreCase;
            }
            else
            {
                comparison = StringComparison.CurrentCulture;
            }

            if (flowDocument == null || string.IsNullOrEmpty(SearchPattern))
            {
                return new SearchResultValue(false, null);
            }

            var textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
            var text = textRange.Text;
            int startIndex = 0;

            // Set the start index to the offset 
            if (StartPosition != null)
            {
                startIndex = textRange.Start.GetOffsetToPosition(StartPosition);
            }

            SearchResultValue searchResult;
            var result = text.IndexOf(SearchPattern, startIndex, comparison);

            // Any result value of 0 or greater indicates a hit
            if (result >= 0)
            {
                // Hit
                var selectionStart = DetermineTextPointer(flowDocument.ContentStart, result);
                var currentSelection = DetermineTextPointer(selectionStart, SearchPattern.Length);

                var selectedRange = new TextRange(selectionStart, currentSelection);

                searchResult = new SearchResultValue(true, selectedRange);
            }
            else
            {
                // No hit
                searchResult = new SearchResultValue(false, null);
            }

            return searchResult;
        }

        private TextPointer DetermineTextPointer(TextPointer start, int count)
        {
            // Iterate over the index positions until the end of the search pattern. Is there a better way of doing it?
            var currentSelection = start.GetPositionAtOffset(count);
            for (int run = 0; run < count; run++)
            {
                currentSelection = currentSelection.GetNextInsertionPosition(LogicalDirection.Forward);
            }

            return currentSelection;
        }

        /// <summary>
        /// Specifies the result of the search operation
        /// </summary>
        /// <param name="Successful">true if any content has been found</param>
        /// <param name="start">the start index</param>
        /// <param name="length">the length</param>
        internal record struct SearchResultValue(bool Successful, TextRange? Selection);
    }
}

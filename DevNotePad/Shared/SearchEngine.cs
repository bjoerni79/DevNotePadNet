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

            //
            //  Define the start position 
            //

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

            var text = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd).Text;
            int startIndex = 0;
            if (StartPosition != null)
            {
                //TODO
            }

            SearchResultValue searchResult;
            var result = text.IndexOf(SearchPattern, startIndex, comparison);
            if (result > 0)
            {
                //TODO: How to transform the string coordinates to a TextPointer?
                var selectionStart = flowDocument.ContentStart.GetPositionAtOffset(result);
                var selectionEnd = flowDocument.ContentStart.GetPositionAtOffset(result).GetPositionAtOffset(SearchPattern.Length);

                searchResult = new SearchResultValue(true, new TextRange(selectionStart,selectionEnd));
            }
            else
            {
                searchResult = new SearchResultValue(false, null);
            }

            return searchResult;
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

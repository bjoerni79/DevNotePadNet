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

            return FindPattern(SearchPattern, comparison, flowDocument, StartPosition);
        }

        private SearchResultValue FindPattern(string searchPattern, StringComparison comparison, FlowDocument document, TextPointer? startPosition)
        {
            SearchResultValue searchResult = new SearchResultValue(false, null);

            // If the start position is defined, start with current selected paragraph
            Block currentBlock;
            if (startPosition != null)
            {
                var currentParagraph = startPosition.Paragraph;
                currentBlock = currentParagraph;
            }
            else
            {
                // Use the first paragraph
                currentBlock = document.Blocks.FirstBlock;
            }

            while (currentBlock != null)
            {
                //TODO: Section
                var paragraph = currentBlock as Paragraph;
                if (paragraph != null)
                {
                    //
                    //  Get the range of the paragraph content and search for the pattern
                    //
                    var paragraphRange = new TextRange(paragraph.ContentStart, paragraph.ContentEnd);
                    var paragraphText = paragraphRange.Text;
                    int result = paragraphText.IndexOf(searchPattern, comparison);
                    if (result >= 0)
                    {
                        //
                        // Hit. Build the search result and leave the for each loop
                        //
                        var startSelection = paragraph.ContentStart.GetPositionAtOffset(result+1);
                        var endSelection = startSelection.GetPositionAtOffset(searchPattern.Length);
                        searchResult = new SearchResultValue(true, new TextRange(startSelection,endSelection));

                        //TODO: Update the StartPosition for Find Next

                        break;
                    }
                }

                currentBlock = currentBlock.NextBlock;
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

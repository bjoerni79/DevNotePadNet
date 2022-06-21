using System;
using System.Windows.Documents;

namespace DevNotePad.Shared
{
    internal class SearchEngine
    {
        private TextPointer? internalSearchPosition;

        internal string? SearchPattern { get; set; }

        internal bool IgnoreLetterType { get; set; }

        internal TextPointer? StartPosition { get; set; }

        internal SearchEngine()
        {
            internalSearchPosition = null;
        }

        internal SearchResultValue RunSearch(FlowDocument flowDocument, bool findNext)
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

            return FindPattern(SearchPattern, comparison, flowDocument, StartPosition, findNext);
        }

        private SearchResultValue FindPattern(string searchPattern, StringComparison comparison, FlowDocument document, TextPointer? startPosition, bool findNext)
        {
            SearchResultValue searchResult = new SearchResultValue(false, null);

            // If the start position is defined, start with current selected paragraph
            Block currentBlock;

            if (findNext && internalSearchPosition != null)
            {
                currentBlock = internalSearchPosition.Paragraph;
            }
            else if (startPosition != null)
            {
                //
                //  Start is defined. This overrides all previous settings
                //
                var currentParagraph = startPosition.Paragraph;
                currentBlock = currentParagraph;
                internalSearchPosition = startPosition;
            }
            else
            {
                // Use the first paragraph
                var blocks = document.Blocks;
                currentBlock = blocks.FirstBlock;

                // TODO: Returns null in some cases!
                internalSearchPosition = currentBlock.ContentStart;
            }

            while (currentBlock != null)
            {
                // Search in paragraphs
                var paragraph = currentBlock as Paragraph;
                if (paragraph != null)
                {
                    //
                    //  Get the range of the paragraph content and search for the pattern
                    //
                    var paragraphRange = new TextRange(internalSearchPosition, paragraph.ContentEnd);
                    var paragraphText = paragraphRange.Text;
                    int result = paragraphText.IndexOf(searchPattern, comparison);
                    if (result >= 0)
                    {
                        //
                        // Hit. Build the search result and leave the for each loop
                        //
                        var startSelection = paragraph.ContentStart.GetPositionAtOffset(result + 1);
                        var endSelection = startSelection.GetPositionAtOffset(searchPattern.Length);
                        searchResult = new SearchResultValue(true, new TextRange(startSelection, endSelection));

                        // Save the current position
                        internalSearchPosition = endSelection;

                        break;
                    }
                }

                // Go to the next block or leave the loop when NextBlock is null (end of paragraphs). Also reset the internal search position if required.
                currentBlock = currentBlock.NextBlock;
                if (currentBlock != null)
                {
                    internalSearchPosition = currentBlock.ContentStart;
                }
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

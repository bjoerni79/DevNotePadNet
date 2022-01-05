using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Shared
{
    internal class SearchEngine
    {
        internal string? SearchPattern { get; set; }

        internal bool IgnoreLetterType { get; set; }

        internal int StartIndex { get; set; }

        internal SearchEngine()
        {
        }

        internal SearchResultValue RunSearch(string text)
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

            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(SearchPattern))
            {
                return new SearchResultValue(false, 0, 0);
            }

            int length = SearchPattern.Length;
            bool patternFound = false;
            var result = text.IndexOf(SearchPattern, StartIndex, comparison);
            if (result > 0)
            {
                patternFound = true;
                StartIndex = result + length;
            }

            return new SearchResultValue(patternFound, result, length);
        }

        /// <summary>
        /// Specifies the result of the search operation
        /// </summary>
        /// <param name="Successful">true if any content has been found</param>
        /// <param name="start">the start index</param>
        /// <param name="length">the length</param>
        internal record struct SearchResultValue(bool Successful, int StartIndex, int Length);
    }
}

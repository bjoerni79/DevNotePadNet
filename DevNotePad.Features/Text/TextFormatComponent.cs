using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Text
{
    internal class TextFormatComponent : ITextFormatComponent
    {

        internal TextFormatComponent()
        {

        }

        public string CountLength(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            var length = text.Length;
            return String.Format("Length Selected : {0} Dec / 0x{1:X2} Hex", length, length);
        }

        public string GroupString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            var filteredText = FilterContent(text);

            if (filteredText.Count() > 0)
            {
                return filteredText;
            }
            else
            {
                return String.Empty;
            }
        }

        public string SplitString(string text)
        {
            return SplitString(text, new TextFormatComponentSettings());
        }

        public string SplitString(string text, TextFormatComponentSettings settings)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var groupCount = settings.GroupCount;
            var filteredText = FilterContent(text);

            if (filteredText.Count() > 0)
            {
                // Split the text into groups of groupCount. 
                var splittedText = new StringBuilder();
                while (filteredText.Count() > groupCount)
                {
                    var group = filteredText.Substring(0, groupCount);
                    filteredText = filteredText.Substring(groupCount);

                    splittedText.AppendFormat("{0}  ", group);
                }

                splittedText.Append(filteredText);
                return splittedText.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        public string ToLower(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length > 0)
            {
                return text.ToLower();
            }
            else
            {
                return String.Empty;
            }
        }

        public string ToUpper(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length > 0)
            {
                return text.ToUpper();
            }
            else
            {
                return String.Empty;
            }
        }

        public string Trim(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length > 0)
            {
                return text.Trim();
            }
            else
            {
                return String.Empty;
            }
        }

        private string FilterContent(string text)
        {
            var filterText = new StringBuilder();
            foreach (var currentChar in text)
            {
                var ignore = Char.IsControl(currentChar) || Char.IsWhiteSpace(currentChar);
                if (!ignore)
                {
                    filterText.Append(currentChar);
                }
            }

            return filterText.ToString();
        }
    }
}

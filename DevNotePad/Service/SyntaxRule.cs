using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DevNotePad.Service
{
    /// <summary>
    /// Specifies basic rules for a first syntax highlightning
    /// </summary>
    public class SyntaxRule
    {
        /// <summary>
        /// Creates a rule for keywords 
        /// </summary>
        /// <param name="keyword">the keyword</param>
        /// <param name="brush">the color</param>
        public SyntaxRule(string keyword, Brush brush)
        {
            KeyWord = keyword;
            IsSymbol = false;
            Brush = brush;
        }

        /// <summary>
        /// Creates a rule for symbols
        /// </summary>
        /// <param name="symbol">the symbol i.e. ';'</param>
        /// <param name="brush">the color</param>
        public SyntaxRule(char symbol, Brush brush)
        {
            KeyWord = symbol.ToString();
            IsSymbol = true;
            Brush = brush;
        }

        /// <summary>
        /// Gets the key word
        /// </summary>
        public string KeyWord { get; private set; }

        /// <summary>
        /// True if the keyword is specified as a symbol only
        /// </summary>
        public bool IsSymbol { get; private set; }

        /// <summary>
        /// Indicates if a rule is currently processing. I.e. the rule is 'keyword' and 'ke'...y is in process
        /// </summary>
        public bool Hit { get; private set; }

        public bool Match { get; private set; }

        public int CurrentPosition { get; private set; }

        /// <summary>
        /// The brush for the action
        /// </summary>
        public Brush Brush { get; private set; }

        public void Test(char curChar)
        {
            bool result;
            var position = CurrentPosition;

            if (IsSymbol)
            {
                // There is only one character. I.e. ";"
                result = CheckForSymbol(curChar);
            }
            else
            {
                if (CurrentPosition < KeyWord.Length)
                {
                    // The key word contains more than one character. i.e. "Public"
                    bool isLastPosition = position == KeyWord.Length - 1;

                    Hit = KeyWord[CurrentPosition] == curChar;
                    result = isLastPosition && Hit;
                }
                else
                {
                    Reset();

                    result = false;
                }
            }

            CurrentPosition++;
            Match =  result;
        }

        private bool CheckForSymbol(char curChar)
        {
            if (CurrentPosition == 0)
            {
                Hit = KeyWord[0] == curChar;
            }
            else
            {
                Reset();
            }

            return Hit;
        }

        public void Reset()
        {
            Hit = false;
            Match = false;
            CurrentPosition = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("SyntaxRule: ");
            sb.AppendFormat("Keyword: {0}, IsSymbol: {1}, Brush: {2}", KeyWord,IsSymbol,Brush);

            return sb.ToString();
        }
    }
}

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
        public bool Hit { get; set; }

        /// <summary>
        /// The brush for the action
        /// </summary>
        public Brush Brush { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("SyntaxRule: ");
            sb.AppendFormat("Keyword: {0}, IsSymbol: {1}, Brush: {2}", KeyWord,IsSymbol,Brush);

            return sb.ToString();
        }
    }
}

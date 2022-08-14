using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace DevNotePad.Service
{
    public class SyntaxHighlightningService : ISyntaxHighlightningService
    {
        private bool isActive;

        public SyntaxHighlightningService()
        {
            // Always on for now. Helps debugging.
            isActive = true;
        }

        public void Refresh()
        {
            if (isActive)
            {
                //TODO: Iterate over all paragraphs...

            }
        }

        public void Refresh(Paragraph paragraph)
        {
            if (isActive && paragraph != null)
            {
                var rules = new List<SyntaxRule>()
                {
                    new SyntaxRule("public",Brushes.Blue),
                    new SyntaxRule("class", Brushes.Black)
                };

                // Parse them
                var text = ReadText(paragraph);
                SyntaxRule currentRule = null;
                var formattedInlineList = new List<Inline>();

                bool doProcess = true;
                int positionInKeyword = 0;
                while (doProcess)
                {
                    var curChar = text.First();

                    // Rule checking
                    if (currentRule != null)
                    {
                        // There is one rule currently in process. Check this one first
                    }
                    else
                    {
                        // ...
                    }

                    // Go to the next character
                    text = text.Skip(1).ToString();
                    if (String.IsNullOrEmpty(text))
                    {
                        doProcess = false;
                    }
                }
            }
        }

        private string ReadText(Paragraph paragraph)
        {
            // Add all text to one buffer. 
            var inlines = paragraph.Inlines;
            var contentBuilder = new StringBuilder();

            foreach (var inline in inlines)
            {
                var run = inline as Run;
                if (run != null)
                {
                    contentBuilder.Append(run.Text);
                }

                //TODO: Spans?

            }

            return contentBuilder.ToString();
        }

        public void Start()
        {
            isActive = true;
        }

        public void Stop()
        {
            isActive = false;
        }
    }
}

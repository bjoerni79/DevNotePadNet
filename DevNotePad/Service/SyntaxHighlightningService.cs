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
                var inlines = paragraph.Inlines;
                foreach (var inline in inlines)
                {
                    bool actionRequired = false;

                    var run = inline as Run;
                    if (run != null)
                    {
                        actionRequired = run.Text.Contains("public");
                    }

                    if (actionRequired)
                    {
                        inline.Foreground = Brushes.Blue;
                    }
                    else
                    {
                        inline.Foreground = Brushes.Black;
                    }
                }
            }
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

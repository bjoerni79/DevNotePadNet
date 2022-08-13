using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

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
                //TODO:...


            }
        }

        public void Refresh(Paragraph paragraph)
        {
            if (isActive && paragraph != null)
            {
                var inlines = paragraph.Inlines;
                foreach (var inline in inlines)
                {
                    
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

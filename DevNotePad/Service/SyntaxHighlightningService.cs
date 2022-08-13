using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Service
{
    public class SyntaxHighlightningService : ISyntaxHighlightningService
    {
        private bool isActive;

        public SyntaxHighlightningService()
        {
            isActive = false;
        }

        public void Refresh()
        {

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

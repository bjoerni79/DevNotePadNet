using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DevNotePad.Service
{
    /// <summary>
    /// Interface for the proof of concept for of the first Syntax Highlightning builder
    /// </summary>
    public interface ISyntaxHighlightningService
    {
        /// <summary>
        /// Starts the watcher for syntax highlightning
        /// </summary>
        void Start();

        /// <summary>
        /// Stopps the watcher for sytnax highlightning
        /// </summary>
        void Stop();

        /// <summary>
        /// Forces a refresh
        /// </summary>
        void Refresh();

        void Refresh(Paragraph paragraph);
    }
}

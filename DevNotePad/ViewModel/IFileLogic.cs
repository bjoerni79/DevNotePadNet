using DevNotePad.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    /// <summary>
    /// Contains the logic required for maintaining the states of the content.
    /// </summary>
    public interface IFileLogic
    {
        /// <summary>
        /// The initial text of the load operation
        /// </summary>
        string InitialText { get; set; }

        /// <summary>
        /// The latest time stamp after Load or Save. 
        /// </summary>
        DateTime LatestTimeStamp { get; set; }

        /// <summary>
        /// The current filename
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// The current state
        /// </summary>
        EditorState CurrentState { get; set; }


        void PerformTextAction(TextActionEnum textAction);

        void PerfromClipboardAction(ClipboardActionEnum action);

        bool Save(string targetfilename);

        bool Load(string sourceFilename);

        bool New();

        void Reload();
    }
}

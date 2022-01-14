using DevNotePad.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    /// <summary>
    /// Result of a refactoring: The view model has been exploded and this componented has been extracted as consequence
    /// </summary>
    public interface IFileLogic
    {
        string InitialText { get; set; }

        DateTime LatestTimeStamp { get; set; }

        string FileName { get; set; }

        EditorState CurrentState { get; set; }

        void InternalText(TextActionEnum textAction);

        void PerfromClipboardAction(ClipboardActionEnum action);

        bool InternalSave(string targetfilename);

        bool InternalLoad(string sourceFilename);

        bool InternalNew();

        void InternalReload();
    }
}

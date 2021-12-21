using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Shared
{
    internal enum EditorState
    {
        Unknown,
        Saved,
        Loaded,
        Changed,
        ChangedNew,
        New
    }
}

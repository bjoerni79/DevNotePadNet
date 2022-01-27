using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Shared.Event
{
    public record UpdateStatusBarParameter(string Message, bool IsWarning);

    public record UpdateAsyncProcessState(bool InProgress);

    //public record UpdateFileState();
}

using DevNotePad.Features.Shared;
using System.Collections.Generic;

namespace DevNotePad.Shared.Event
{
    public record UpdateStatusBarParameter(string Message, bool IsWarning);

    public record UpdateAsyncProcessState(bool InProgress);

    public record UpdateTree(IEnumerable<ItemNode> ItemNodes);
}

using CommunityToolkit.Mvvm.Messaging.Messages;
using DevNotePad.Features.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Shared.Message
{
    public class UpdateTreeMessage : ValueChangedMessage<IEnumerable<ItemNode>>
    {
        public UpdateTreeMessage(IEnumerable<ItemNode> nodes) : base(nodes)
        {

        }
    }
}

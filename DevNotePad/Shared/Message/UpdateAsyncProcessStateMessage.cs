using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Shared.Message
{
    public class UpdateAsyncProcessStateMessage : ValueChangedMessage<bool>
    {
        public UpdateAsyncProcessStateMessage(bool isAsync) : base(isAsync)
        {

        }
    }
}

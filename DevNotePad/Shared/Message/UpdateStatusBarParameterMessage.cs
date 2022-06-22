using CommunityToolkit.Mvvm.Messaging.Messages;
using DevNotePad.Shared.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Shared.Message
{
    public class UpdateStatusBarParameterMessage : ValueChangedMessage<UpdateStatusBarParameter>
    {
        public UpdateStatusBarParameterMessage(UpdateStatusBarParameter parameter) : base(parameter)
        {

        }
    }
}

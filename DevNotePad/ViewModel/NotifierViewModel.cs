using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class NotifierViewModel : ObservableRecipient
    {
        public NotifierViewModel()
        {

        }

        //TODO: Events...

        public string? Message { get; private set; }
    }
}

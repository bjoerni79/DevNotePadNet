using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    internal class TextFormatterSetting
    {
        internal TextFormatterSetting()
        {
            GroupCount = 4;
        }

        internal int GroupCount { get; set; }
    }
}

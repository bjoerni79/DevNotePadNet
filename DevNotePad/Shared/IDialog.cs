using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Shared
{
    public interface IDialog
    {
        void CloseDialog(bool confirmed);
    }
}

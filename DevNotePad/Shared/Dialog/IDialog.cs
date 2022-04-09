using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    public interface IDialog
    {
        Window GetCurrentWindow();

        void CloseDialog(bool confirmed);
    }
}

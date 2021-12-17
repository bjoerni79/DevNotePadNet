using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Service
{
    public interface IDialogService
    {
        DialogReturnValue ShowOpenFileNameDialog(string title, string defaultExtension, string searchPattern);

        DialogReturnValue ShowSaveFileDialog();

        void ShowErrorDialog(Exception ex);

        void ShowWarningDialog(string warning, string caption);
    }
}

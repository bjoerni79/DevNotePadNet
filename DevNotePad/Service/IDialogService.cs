using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Service
{
    public interface IDialogService
    {
        string ShowOpenFileNameDialog();

        string ShowSaveFileDialog();

        void ShowErrorDialog(Exception ex);

        void ShowWarningDialog(string warning, string caption);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Service
{
    internal class DialogService : IDialogService
    {
        internal DialogService()
        {
            //TODO: Add the reference to the main window!
        }

        public void ShowErrorDialog(Exception ex)
        {
            throw new NotImplementedException();
        }

        public string ShowOpenFileNameDialog()
        {
            throw new NotImplementedException();
        }

        public string ShowSaveFileDialog()
        {
            throw new NotImplementedException();
        }

        public void ShowWarningDialog(string warning, string caption)
        {
            throw new NotImplementedException();
        }
    }
}

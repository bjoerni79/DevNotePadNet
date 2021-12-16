using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevNotePad.Service
{
    internal class DialogService : IDialogService
    {
        private Window owner;

        internal DialogService(Window owner)
        {
            this.owner = owner;
        }

        public void ShowErrorDialog(Exception ex)
        {
            MessageBox.Show("Error" + ex.Message);
        }

        public string ShowOpenFileNameDialog(string title, string defaultExtension, string searchPattern)
        {
            var openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog(owner);

            if (result.HasValue && result.Value)
            {
                return openFileDialog.FileName;
            }

            return null;
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

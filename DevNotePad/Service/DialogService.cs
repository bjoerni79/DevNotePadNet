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

        public DialogReturnValue ShowOpenFileNameDialog(string title, string defaultExtension, string searchPattern)
        {
            var openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog(owner);

            if (result.HasValue && result.Value)
            {
                return new DialogReturnValue(true, openFileDialog.FileName);
            }

            return new DialogReturnValue(false, String.Empty);
        }

        public DialogReturnValue ShowSaveFileDialog()
        {
            var saveFileDialog = new SaveFileDialog();
            var result = saveFileDialog.ShowDialog(owner);

            if (result.HasValue && result.Value)
            {
                return new DialogReturnValue(true, saveFileDialog.FileName);
            }

            return new DialogReturnValue(false, String.Empty);
        }

        public void ShowWarningDialog(string warning, string caption)
        {
            MessageBox.Show(warning, caption);
        }
    }
}

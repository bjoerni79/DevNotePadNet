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

        public void ShowErrorDialog(Exception ex, string component)
        {
            //MessageBox.Show("Error" + ex.Message);
            var errorDialog = new OkDialog();
            errorDialog.Component = component;
            errorDialog.Message = ex.Message;
            errorDialog.DialogTitle = "Error";
            errorDialog.Owner = owner;

            errorDialog.ShowDialog();
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

        public void ShowWarningDialog(string warning, string component)
        {
            //MessageBox.Show(warning, caption);

            var errorDialog = new OkDialog();
            errorDialog.Component = component;
            errorDialog.Message = warning;
            errorDialog.DialogTitle = "Warning";
            errorDialog.Owner = owner;

            errorDialog.ShowDialog();
        }
    }
}

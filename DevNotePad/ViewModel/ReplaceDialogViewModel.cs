using DevNotePad.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class ReplaceDialogViewModel : AbstractViewModel
    {
        private IMainViewUi ui;
        private IDialog dialog;

        public ReplaceDialogViewModel(IMainViewUi mainViewUi, IDialog dialogUi)
        {
            ui = mainViewUi;
            dialog = dialogUi;
        }


    }
}

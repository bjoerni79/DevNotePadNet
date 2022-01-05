using DevNotePad.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class MainViewUiViewModel : AbstractViewModel
    {
        protected IMainViewUi ui;
        protected IDialog dialog;

        public void Init(IMainViewUi mainViewUi, IDialog dialog)
        {
            this.ui = mainViewUi;
            this.dialog = dialog;
        }
    }
}

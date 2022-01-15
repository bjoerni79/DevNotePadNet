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
        protected ITextComponent textComponent;

        public void Init(IMainViewUi mainViewUi, IDialog dialog, ITextComponent textComponent)
        {
            this.ui = mainViewUi;
            this.dialog = dialog;
            this.textComponent = textComponent;
        }
    }
}

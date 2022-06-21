using CommunityToolkit.Mvvm.ComponentModel;
using DevNotePad.Shared.Dialog;

namespace DevNotePad.ViewModel
{
    public class MainViewUiViewModel : ObservableObject
    {
        protected IMainViewUi? ui;
        protected IDialog? dialog;
        protected ITextComponent? textComponent;

        protected XmlToolHelper? xmlToolHelper;

        public void Init(IMainViewUi mainViewUi, IDialog dialog, ITextComponent textComponent)
        {
            this.ui = mainViewUi;
            this.dialog = dialog;
            this.textComponent = textComponent;

            xmlToolHelper = new XmlToolHelper(dialog);
        }

        public void Init(IDialog dialog)
        {
            this.dialog = dialog;
        }

    }
}

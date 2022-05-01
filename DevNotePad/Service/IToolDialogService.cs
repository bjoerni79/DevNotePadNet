using DevNotePad.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Service
{
    public interface IToolDialogService
    {
        void OpenFindDialog(IMainViewUi ui, ITextComponent textComponent);

        void OpenReplaceDialog(IMainViewUi ui, ITextComponent textComponent);

        void OpenBase64Dialog(IMainViewUi ui, ITextComponent textComponent);

        void OpenAppletToolDialog(IMainViewUi ui, ITextComponent textComponent);

        void OpenXmlSchemaValidatorDialog(IMainViewUi ui, ITextComponent textComponent);

        void OpenXPathQueryDialog(IMainViewUi ui, ITextComponent textComponent);

        void OpenXSltTransfomerDialog(IMainViewUi ui, ITextComponent textComponent);

        void OpenRegularExpressionDialog (IMainViewUi ui, ITextComponent textComponent);

        void OpenGuidDialog(IMainViewUi ui, ITextComponent textComponent);

        void OpenTreeView();

    }
}

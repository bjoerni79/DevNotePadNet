using System.Windows;

namespace DevNotePad.Shared.Dialog
{
    public interface IDialog
    {
        Window GetCurrentWindow();

        void CloseDialog(bool confirmed);
    }
}

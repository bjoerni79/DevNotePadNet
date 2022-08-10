namespace DevNotePad.ViewModel
{
    public interface IMainViewModel
    {
        void Init(IMainViewUi ui, ITextComponent text);

        void NotifyContentChanged(int added, int offset, int removed);

        void ApplySettings();

        void OpenExternalFile(string filePath);

        bool IsChanged();
    }
}

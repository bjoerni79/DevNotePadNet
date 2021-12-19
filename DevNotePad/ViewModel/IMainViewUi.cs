using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public interface IMainViewUi
    {
        void SetScrollbars(bool enable);
        void SetWordWrap(bool enable);

        void ShowAbout();

        string GetText(bool selected);

        void SetText(string text);

        void SetText(string text, bool selected);

        bool IsTextSelected();

        void AddToScratchPad(string text);
    }
}

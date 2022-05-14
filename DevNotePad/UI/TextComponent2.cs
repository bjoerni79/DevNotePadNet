using DevNotePad.Shared;
using DevNotePad.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DevNotePad.UI
{
    public class TextComponent2 : ITextComponent
    {
        private RichTextBox richTextBox;

        public TextComponent2(RichTextBox instance)
        {
            richTextBox = instance;
        }

        public void AddText(string text)
        {
            throw new NotImplementedException();
        }

        public int GetCurrentPosition()
        {
            throw new NotImplementedException();
        }

        public string GetText(bool selected)
        {
            throw new NotImplementedException();
        }

        public bool IsTextSelected()
        {
            throw new NotImplementedException();
        }

        public void PerformClipboardAction(ClipboardActionEnum action)
        {
            throw new NotImplementedException();
        }

        public void SelectText(int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public void SetText(string text)
        {
            throw new NotImplementedException();
        }

        public void SetText(string text, bool selected)
        {
            throw new NotImplementedException();
        }
    }
}

using DevNotePad.Shared;
using DevNotePad.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DevNotePad.UI
{
    public class TextComponent2 : ITextComponent
    {
        private DevTextBox2 _editorControl;
        private FlowDocument document;


        // https://stackoverflow.com/questions/3934422/wpf-richtextbox-get-whole-word-at-current-caret-position

        public TextComponent2(DevTextBox2 editorControl)
        {
            _editorControl = editorControl;
            document = editorControl.Document;
        }

        public void AddText(string text)
        {
            // Add the text at current position.. 
            // editor.AppendText("\n" + text);

            var endPointer = document.ContentEnd;
            endPointer.InsertTextInRun(text);
        }

        public TextPointer GetCurrentPosition()
        {
            //return editor.CaretIndex;
            var position = _editorControl.CaretPosition;

            return position;
        }

        public string GetText(bool selected)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.textrange?view=windowsdesktop-6.0

            //App.Current.Dispatcher.Invoke(() =>
            //{

            //});

            string content;
            if (selected)
            {
                // Return only the selected text
                var selection = _editorControl.Selection;

                content = selection.Text;
            }
            else
            {
                // Return the entire text
                var helper = new FlowDocumentHelper();
                content = helper.Extract(document);
            }

            return content;
        }

        public bool IsTextSelected()
        {
            // True / False...

            var selection = _editorControl.Selection;
            var isEmptySelection = selection.IsEmpty;

            return !isEmptySelection;
        }

        public void PerformClipboardAction(ClipboardActionEnum action)
        {
            //switch (action)
            //{
            //    case ClipboardActionEnum.Copy:
            //        editor.Copy();
            //        break;
            //    case ClipboardActionEnum.Paste:
            //        editor.Paste();
            //        break;
            //    case ClipboardActionEnum.Cut:
            //        editor.Cut();
            //        break;
            //    case ClipboardActionEnum.SelectAll:
            //        editor.SelectAll();
            //        break;
            //}
        }

        public void SelectText(TextPointer startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public void SetText(string text)
        {
            SetText(text, false);
        }

        public void SetText(string text, bool selected)
        {
            // throw new NotImplementedException();

            // App.Current.Dispatcher.Invoke(() => {  });
        }
    }
}

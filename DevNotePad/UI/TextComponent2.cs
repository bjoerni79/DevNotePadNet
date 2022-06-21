using DevNotePad.Shared;
using DevNotePad.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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
            if (endPointer.IsAtInsertionPosition)
            {
                endPointer.InsertTextInRun(text);
            }
            else
            {
                var nextValid = endPointer.GetNextInsertionPosition(LogicalDirection.Forward);
                nextValid.InsertTextInRun(text);
            }
        }

        public TextPointer GetCurrentPosition()
        {
            var currentPos = _editorControl.CaretPosition;
            var isAtInsertPos = currentPos.IsAtInsertionPosition;
            if (isAtInsertPos)
            {
                return currentPos;
            }

            return currentPos.GetNextInsertionPosition(LogicalDirection.Forward);
        }

        public FlowDocument GetDocument()
        {
            return _editorControl.Document;
        }

        public TextPointer GetStartPosition()
        {
            return document.ContentStart;
        }

        public string GetText(bool selected)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.textrange?view=windowsdesktop-6.0

            string content;
            if (selected)
            {
                // Return only the selected text
                var selection = _editorControl.Selection;
                if (selection != null && !selection.IsEmpty)
                {
                    content = selection.Text;
                }
                else
                {
                    content = String.Empty;
                }
            }
            else
            {
                // Return the entire text
                var textRange = new TextRange(document.ContentStart, document.ContentEnd);
                content = textRange.Text;
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
            switch (action)
            {
                case ClipboardActionEnum.Copy:
                    _editorControl.Copy();
                    break;
                case ClipboardActionEnum.Paste:
                    _editorControl.Paste();
                    break;
                case ClipboardActionEnum.Cut:
                    _editorControl.Cut();
                    break;
                case ClipboardActionEnum.SelectAll:
                    _editorControl.SelectAll();
                    break;
            }
        }

        public void SelectText(TextRange range)
        {
            if (range != null && !range.IsEmpty)
            {
                var textRange = _editorControl.Selection;

                textRange.Select(range.Start, range.End);
                _editorControl.Focus();
            }
        }

        public void SetText(string text)
        {
            SetText(text, false);
        }

        public void SetText(string text, bool selected)
        {
            App.Current.Dispatcher.Invoke(() => { 
                if (selected)
                {
                    if (_editorControl.Selection != null && !_editorControl.Selection.IsEmpty)
                    _editorControl.Selection.Text = text;
                }
                else
                {
                    // Create new flow document in this case?  All content gets overwritten
                    var newDocument = new FlowDocument();
                    newDocument.LineHeight = Double.NaN;

                    // Populate the new document and remove the first empty one
                    if (text.Any())
                    {
                        var lines = text.Split(Environment.NewLine);
                        foreach (var line in lines)
                        {
                            var p = new Paragraph(new Run(line));
                            newDocument.Blocks.Add(p);
                        }

                        newDocument.Blocks.Remove(newDocument.Blocks.FirstBlock);
                    }

                    _editorControl.Document = newDocument;
                }
            });
        }
    }
}

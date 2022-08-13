using DevNotePad.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DevNotePad.UI
{
    public class DevTextBox2 : RichTextBox
    {
        public static readonly DependencyProperty CurrentRowProperty = DependencyProperty.Register(
            "CurrentRow", typeof(int),
            typeof(DevTextBox2));

        public static readonly DependencyProperty CurrentColumnProperty = DependencyProperty.Register(
            "CurrentColumn", typeof(int),
            typeof(DevTextBox2));

        public DevTextBox2() : base()
        {
            // Empty
        }

        /// <summary>
        /// Contains the current Row of the caret
        /// </summary>
        public int CurrentRow
        {
            get => (int)GetValue(CurrentRowProperty);
            set => SetValue(CurrentRowProperty, value);
        }

        /// <summary>
        /// Contains the current column of the caret
        /// </summary>
        public int CurrentColumn
        {
            get => (int)GetValue(CurrentColumnProperty);
            set => SetValue(CurrentColumnProperty, value);
        }

        private void UpdatePosition()
        {
            var caretPosition = CaretPosition;
            var paragraph = caretPosition.Paragraph;

            // Search for the paragraph in the blocks
            bool lineFound = false;
            int lineCount = 1;
            if (paragraph != null)
            {
                var blocks = Document.Blocks;
                foreach (var currentBlock in blocks)
                {
                    var curParagraph = currentBlock as Paragraph;
                    if (curParagraph != null && curParagraph == paragraph)
                    {
                        lineFound = true;
                        break;
                    }

                    lineCount++;
                }
            }

            if (lineFound)
            {
                CurrentRow = lineCount;

                // Update column
                var startPos = paragraph.ContentStart;
                var diff = startPos.GetOffsetToPosition(CaretPosition);
                CurrentColumn = diff;
            }

            // Trigger the syntax check

            var syntaxHighligherService = App.Current.BootStrap.Services.GetService<ISyntaxHighlightningService>();
            syntaxHighligherService.Refresh(paragraph);
        }

        #region Protected 

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            UpdatePosition();
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            var isKeyMovement = e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down;
            var isSpecialKey = e.Key == Key.PageUp || e.Key == Key.PageDown || e.Key == Key.Home || e.Key == Key.End;
            if (isKeyMovement || isSpecialKey)
            {
                UpdatePosition();
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            UpdatePosition();
        }

        #endregion
    }
}

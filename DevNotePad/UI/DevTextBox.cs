using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevNotePad.UI
{
    public class DevTextBox : TextBox
    {
        // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/properties/dependency-properties-overview?view=netdesktop-6.0

        public static readonly DependencyProperty CurrentRowProperty = DependencyProperty.Register(
            "CurrentRow", typeof(int),
            typeof(DevTextBox));

        public static readonly DependencyProperty CurrentColumnProperty = DependencyProperty.Register(
            "CurrentColumn", typeof(int),
            typeof(DevTextBox));

        public DevTextBox() : base()
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
            // Maybe sync with Begin/End...

            var caretIndex = CaretIndex;
            var lineCount = LineCount;

            if (caretIndex < 1)
            {
                // No text

                CurrentColumn = 1;
                CurrentRow = 1;
            }
            else if (lineCount == -1)
            {
                // Unkown state. Just set it to 0

                CurrentRow = 0;
                CurrentColumn = 0;
            }
            else
            {
                // Calculate the current column and row
                int rowAfterHit = -1;
                for (int curLine = 0; curLine < lineCount; curLine++)
                {
                    var firstCharacterindex = GetCharacterIndexFromLineIndex(curLine);

                    if (caretIndex == firstCharacterindex)
                    {
                        rowAfterHit = curLine;
                        break;
                    }

                    if (caretIndex < firstCharacterindex)
                    {
                        // Stop.
                        rowAfterHit = curLine - 1;
                        break;
                    }
                }

                if (rowAfterHit == -1)
                {
                    rowAfterHit = lineCount - 1;
                }

                var startIndex = GetCharacterIndexFromLineIndex(rowAfterHit);

                // It is zero based.
                CurrentRow = rowAfterHit + 1;
                CurrentColumn = caretIndex - startIndex + 1;
            }
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

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


        public DevTextBox() : base()
        {
            // Empty
        }

        //TODO: Convert this to a dependency property and connect via Binding to the label (Element)
        public int CurrentRow { get; private set; }

        //TODO: Convert this to a dependency property and connect via Binding to the label (Element)
        public int CurrentColumn { get; private set; }

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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.LeftButton == MouseButtonState.Released)
            {
                UpdatePosition();
            }
        }

        protected override void OnSelectionChanged(RoutedEventArgs e)
        {
            base.OnSelectionChanged(e);
        }

        private void UpdatePosition()
        {   
            // Maybe sync with Begin/End...

            var caretIndex = CaretIndex;
            var lineCount = LineCount;

            if (caretIndex < 1)
            {
                // No text

                CurrentColumn = 0;
                CurrentRow = 0;
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
                int rowAfterHit=0;
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

                var startIndex = GetCharacterIndexFromLineIndex(rowAfterHit);

                // It is zero based.
                CurrentRow = rowAfterHit + 1;
                CurrentColumn = caretIndex - startIndex;
            }
        }
    }
}

﻿using DevNotePad.Shared;
using DevNotePad.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DevNotePad.UI
{
    /// <summary>
    /// Represents a wrapper for a TextBox implementing the ITextComponent interface
    /// </summary>
    public class TextComponent : ITextComponent
    {
        private TextBox editor;

        public TextComponent(TextBox textBox)
        {
            this.editor = textBox;
         }

        public int GetCurrentPosition()
        {
            return editor.CaretIndex;
        }

        public string GetText(bool selected)
        {
            if (selected)
            {
                var selectedText = editor.SelectedText;
                return selectedText;
            }
            else
            {
                return editor.Text;
            }
        }

        public bool IsTextSelected()
        {
            var selectedText = editor.SelectedText;
            return !string.IsNullOrWhiteSpace(selectedText);
        }

        public void PerformClipboardAction(ClipboardActionEnum action)
        {
            switch (action)
            {
                case ClipboardActionEnum.Copy:
                    editor.Copy();
                    break;
                case ClipboardActionEnum.Paste:
                    editor.Paste();
                    break;
                case ClipboardActionEnum.Cut:
                    editor.Cut();
                    break;
                case ClipboardActionEnum.SelectAll:
                    editor.SelectAll();
                    break;
            }
        }

        public void SelectText(int startIndex, int length)
        {
            editor.Focus();
            editor.Select(startIndex, length);

            //var selectedText = editor.SelectedText;
        }

        public void SetText(string text)
        {
            SetText(text, false);
        }

        public void SetText(string text, bool selected)
        {
            editor.BeginChange();

            if (selected)
            {
                editor.SelectedText = text;
            }
            else
            {
                editor.Text = text;
            }

            editor.EndChange();
        }
    }
}
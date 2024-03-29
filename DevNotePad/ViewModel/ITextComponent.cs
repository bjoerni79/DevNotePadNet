﻿using DevNotePad.Shared;
using System.Windows.Documents;

namespace DevNotePad.ViewModel
{
    /// <summary>
    /// Represents a text component for the view model. 
    /// </summary>
    public interface ITextComponent
    {
        /// <summary>
        /// Returns the text. Depending on the parameter it is the selected one or not
        /// </summary>
        /// <param name="selected">true if the selected text should be returned</param>
        /// <returns>the text</returns>
        string GetText(bool selected);

        /// <summary>
        /// Returns the current caret positon
        /// </summary>
        /// <returns>the current position</returns>
        TextPointer GetCurrentPosition();

        TextPointer GetStartPosition();

        FlowDocument GetDocument();

        /// <summary>
        /// Sets the text of the component
        /// </summary>
        /// <param name="text">the text</param>
        void SetText(string text);

        /// <summary>
        /// Sets the text of the component
        /// </summary>
        /// <param name="text">the text</param>
        /// <param name="selected">true if the selected text should be replaced only</param>
        void SetText(string text, bool selected);

        /// <summary>
        /// Adds some text at the current position
        /// </summary>
        /// <param name="text">the text</param>
        void AddText(string text);

        /// <summary>
        /// Checks if there is a text selection 
        /// </summary>
        /// <returns>true if there is, otherwise false</returns>
        bool IsTextSelected();

        void SelectText(TextRange range);


        /// <summary>
        /// Runs the clipboard action on the textbox
        /// </summary>
        /// <remarks>
        /// This could be done in the logic as well. All modern control implements such a feature and as long as this works well keep it
        /// </remarks>
        /// <param name="action">the action</param>
        void PerformClipboardAction(ClipboardActionEnum action);
    }
}

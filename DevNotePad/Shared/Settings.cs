using System;
using System.IO;
using System.Xml;

namespace DevNotePad.Shared
{
    public class Settings
    {
        /*
         * Version 1.0:
         * ---
         *   EditorFontSize = 12;
         *   DefaultPath = null;
         *   LineWrap = false;
         *   ScratchPadEnabled = false;
         *   EditorFontSize = 12;
         *   IgnoreChanged = true;
         *   IgnoreReload = true;
         *   IgnoreOverwriteChanges = false;
         *   DefaultPath = null;
         * 
         * Version 1.1
         * ---
         * 
         *    DarkModeEnabled true/false
         * 
         */

        /// <summary>
        /// Inits a new settings object with the default values
        /// </summary>
        public Settings()
        {
            EditorFontSize = 12;
            DefaultPath = null;
            LineWrap = false;
            EditorFontSize = 12;
            IgnoreChanged = true;
            IgnoreReload = true;
            IgnoreOverwriteChanges = false;
            DefaultPath = null;
            DarkModeEnabled = false;
        }

        #region Properties

        /// <summary>
        /// Internal release scheme for future updates
        /// </summary>
        public string Version => "1.1";

        public int EditorFontSize { get; set; }

        /// <summary>
        /// If enabled, ignores the changes of any pending files on close or new/load
        /// </summary>
        public bool IgnoreChanged { get; set; }

        /// <summary>
        /// If enabled, ignores the changes on the local copy when performing the reload
        /// </summary>
        public bool IgnoreReload { get; set; }

        /// <summary>
        /// If enabled, ignores the changes on the hard disk created after the file has been opened in the editor (time stamp check)
        /// </summary>
        public bool IgnoreOverwriteChanges { get; set; }

        public string? DefaultPath { get; set; }

        public bool LineWrap { get; set; }

        public bool ScratchPadEnabled { get; set; }

        public bool DarkModeEnabled { get; set; }

        #endregion


    }
}

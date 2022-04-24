using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Shared
{
    public class Settings
    {
        public Settings()
        {

        }

        public static Settings GetDefault()
        {
            var settings = new Settings();
            settings.LineWrap = false;
            settings.ScratchPadEnabled = false;
            settings.EditorFontSize = 12;
            settings.IgnoreChanged = true;
            settings.IgnoreReload = true;
            settings.IgnoreOverwriteChanges = false;
            settings.DefaultPath = null;

            return settings;
        }

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
    }
}

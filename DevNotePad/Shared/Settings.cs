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
            settings.DefaultPath = null;

            return settings;
        }

        public int EditorFontSize { get; set; }

        public bool IgnoreChanged { get; set; }

        public bool IgnoreReload { get; set; }

        public string? DefaultPath { get; set; }

        public bool LineWrap { get; set; }

        public bool ScratchPadEnabled { get; set; }
    }
}

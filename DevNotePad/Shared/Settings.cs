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
            settings.ScrollbarAlwaysOn = false;
            settings.LineWrap = false;
            settings.ScratchPadEnabled = false;

            return settings;
        }

        public bool ScrollbarAlwaysOn { get; set; }

        public bool LineWrap { get; set; }

        public bool ScratchPadEnabled { get; set; }
    }
}

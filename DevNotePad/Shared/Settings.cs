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
            settings.ScrollbarAlwaysOn = true;

            return settings;
        }

        public bool ScrollbarAlwaysOn { get; set; }
    }
}

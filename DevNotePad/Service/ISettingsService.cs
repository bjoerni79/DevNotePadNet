using DevNotePad.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Service
{
    /// <summary>
    /// Manages the settings of ..
    /// </summary>
    public interface ISettingsService
    {
        Settings GetSettings();
        void SetSettings(Settings settings);

        Settings GetDefaultSettings();
    }
}

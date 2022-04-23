using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class SettingsDialogViewModel : MainViewUiViewModel
    {
        public SettingsDialogViewModel()
        {

        }

        public bool LineWrap { get; set; }

        public bool EnableScratchPadView { get; set; }

        public bool IgnoreChanges { get; set; }

        public bool IgnoreChangesOnReload { get; set; }

        public string EditorFontSize { get; set; }

        public string DefaultWorkingPath { get; set; }


    }
}

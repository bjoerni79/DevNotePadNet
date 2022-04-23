using DevNotePad.MVVM;
using DevNotePad.Shared;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevNotePad.ViewModel
{
    public class SettingsDialogViewModel : AbstractViewModel
    {
        public SettingsDialogViewModel()
        {
            Apply = new DefaultCommand(() => Close(true));
            Cancel = new DefaultCommand(() => Close(false));
        }

        #region UI Bindings

        public bool LineWrap { get; set; }

        public bool EnableScratchPadView { get; set; }

        public bool IgnoreChanges { get; set; }

        public bool IgnoreChangesOnReload { get; set; }

        public string? EditorFontSize { get; set; }

        public string? DefaultWorkingPath { get; set; }

        public ICommand? Apply { get; set; }

        public ICommand? Cancel { get; set; }

        #endregion

        /// <summary>
        /// Populates the properties used for binding with the current values in the settings instance
        /// </summary>
        public void Init()
        {
            var facade = ServiceHelper.GetFacade();
            var settings = facade.Get<Settings>(Bootstrap.SettingsId);

            // Populate the properties for the UI bindings now
            LineWrap = settings.LineWrap;
            EnableScratchPadView = settings.ScratchPadEnabled;
            IgnoreChanges = settings.IgnoreChanged;
            IgnoreChangesOnReload = settings.IgnoreReload;
            DefaultWorkingPath = settings.DefaultPath;
            EditorFontSize = settings.EditorFontSize.ToString();

            // No Update required. Gets loaded before UI is active.
        }

        private void Close(bool apply)
        {
            if (apply)
            {
                // Write the value back to the settings 
                var facade = ServiceHelper.GetFacade();
                var settings = facade.Get<Settings>(Bootstrap.SettingsId);

                settings.LineWrap = LineWrap;
                settings.ScratchPadEnabled = EnableScratchPadView;
                settings.IgnoreChanged = IgnoreChanges;
                settings.IgnoreReload = IgnoreChangesOnReload;
                settings.DefaultPath = DefaultWorkingPath;
                // Font Size is TODO

            }

            //TODO: How to close this?  Can I reuse an Interface?
        }
    }
}

﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared;
using DevNotePad.Shared.Dialog;
using Microsoft.Extensions.DependencyInjection;

namespace DevNotePad.ViewModel
{
    public class SettingsDialogViewModel : ObservableObject
    {
        private IDialog? dialogInstance;

        public SettingsDialogViewModel()
        {
            Apply = new RelayCommand(() => Close(true));
            Cancel = new RelayCommand(() => Close(false));
        }

        #region UI Bindings

        public bool LineWrap { get; set; }


        public bool IgnoreChanges { get; set; }

        public bool IgnoreChangesOnReload { get; set; }

        public bool IgnoreOverwriteChanges { get; set; }

        public bool DarkMode { get; set; }

        public string? EditorFontSize { get; set; }

        public string? DefaultWorkingPath { get; set; }

        public RelayCommand? Apply { get; set; }

        public RelayCommand? Cancel { get; set; }

        #endregion

        /// <summary>
        /// Populates the properties used for binding with the current values in the settings instance
        /// </summary>
        public void Init(IDialog dialog)
        {
            dialogInstance = dialog;

            Populate();
        }

        private void Close(bool apply)
        {
            if (apply)
            {
                ApplyValues();

            }

            if (dialogInstance != null)
            {
                dialogInstance.CloseDialog(apply);
            }
        }

        private void Populate()
        {
            var settingsService = App.Current.BootStrap.Services.GetService<ISettingsService>();
            if (settingsService != null)
            {
                var settings = settingsService.GetSettings();

                // Populate the properties for the UI bindings now
                LineWrap = settings.LineWrap;
                IgnoreChangesOnReload = settings.IgnoreReload;
                IgnoreOverwriteChanges = settings.IgnoreOverwriteChanges;
                DefaultWorkingPath = settings.DefaultPath;
                DarkMode = settings.DarkModeEnabled;
                EditorFontSize = settings.EditorFontSize.ToString();

                // No Update required.Gets loaded before UI is active.
            }
        }

        private void ApplyValues()
        {
            var settingsService = App.Current.BootStrap.Services.GetService<ISettingsService>();
            if (settingsService != null)
            {
                var settings = settingsService.GetSettings();
                settings.LineWrap = LineWrap;
                settings.IgnoreChanged = IgnoreChanges;
                settings.IgnoreReload = IgnoreChangesOnReload;
                settings.IgnoreOverwriteChanges = IgnoreOverwriteChanges;
                settings.DarkModeEnabled = DarkMode;
                settings.DefaultPath = DefaultWorkingPath;
                // Font Size is TODO

                // Write it to disk
                settingsService.SetSettings(settings);
            }
        }
    }
}

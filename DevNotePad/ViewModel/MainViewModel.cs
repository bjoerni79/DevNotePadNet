using DevNotePad.Feature;
using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared;
using Generic.MVVM;
using Generic.MVVM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class MainViewModel : AbstractViewModel, IMainViewModel
    {
        private IMainViewUi? Ui { get; set; }

        private bool isFilenameAvailable = false;

        public MainViewModel()
        {
            Ui = null;

            // File
            New = new DefaultCommand(OnNew);
            Open = new DefaultCommand(OnOpen);
            Save = new DefaultCommand(OnSave);
            SaveAs = new DefaultCommand(OnSaveAs);
            Reload = new DefaultCommand(OnReload);

            //Tools
            JsonFormatter = new DefaultCommand(OnJsonFormatter);

            // Layout
            ToggleLineWrap = new DefaultCommand(OnToggleTextWrap);
            ToggleScrollbar = new DefaultCommand(OnToggleScrollbar);

            // About
            About = new DefaultCommand(OnAbout);

        }

        #region Commands

        public IRefreshCommand New { get; set; }

        public IRefreshCommand Open { get; set; }

        public IRefreshCommand Save { get; set; }

        public IRefreshCommand SaveAs { get; set; }

        public IRefreshCommand Reload { get; set; }

        public IRefreshCommand ToggleScrollbar { get; set; }
        public IRefreshCommand ToggleLineWrap { get; set; }

        public IRefreshCommand JsonFormatter { get; set; }

        public IRefreshCommand About { get; set; }

        #endregion

        public string? Text { get; set; }

        public string? FileName { get; set; }

        public bool LineWrapMode { get; set; }

        public bool ScrollbarMode { get; set; }

        private void OnNew()
        {
            //TODO: Ask for confirmation not saved!

            InternalNew();
        }

        private void OnReload()
        {
            //TODO
        }

        private void OnOpen()
        {
            var dialogService = GetDialogService();
            var result = dialogService.ShowOpenFileNameDialog("Open New File", "*.txt", String.Empty);

            if (result.IsConfirmed)
            {
                InternalLoad(result.File);
            }
        }

        private void OnSave()
        {
            if (!isFilenameAvailable)
            {
                OnSaveAs();
            }

            // Just save it
            InternalSave(FileName);
        }

        private void OnSaveAs()
        {
            var dialogService = GetDialogService();
            var result = dialogService.ShowSaveFileDialog();

            if (result.IsConfirmed)
            {
                InternalSave(result.File);
            }
        }

        private void OnToggleScrollbar()
        {
            var settings = GetSettings();

            var scrollbarMOde = settings.ScrollbarAlwaysOn;
            ScrollbarMode = !scrollbarMOde;

            settings.ScrollbarAlwaysOn = ScrollbarMode;
            RaisePropertyChange("ScrollbarMode");
            ApplySettings();
        }

        private void OnToggleTextWrap()
        {
            var settings = GetSettings();

            var lineWrap = settings.LineWrap;
            LineWrapMode = !lineWrap;

            settings.LineWrap = LineWrapMode;
            RaisePropertyChange("LineWrapMode");
            ApplySettings();
        }

        private void OnJsonFormatter()
        {
            var input = Text;
            try
            {
                IJsonComponent jsonComponent = new JsonComponent();
                var result = jsonComponent.Formatter(input);

                Text = result;
                RaisePropertyChange("Text");
            }
            catch (FeatureException featureException)
            {
                ShowError(featureException);
            }
        }

        private void OnAbout()
        {
            if (Ui != null)
            {
                Ui.ShowAbout();
            }
        }

        #region IMainViewModel

        public void Init(IMainViewUi ui)
        {
            Ui = ui;
            InternalNew();
        }

        public void ApplySettings()
        {
            var settings = GetSettings();

            if (Ui != null)
            {
                ScrollbarMode = settings.ScrollbarAlwaysOn;
                LineWrapMode = settings.LineWrap;

                Ui.SetScrollbars(ScrollbarMode);
                Ui.SetWordWrap(LineWrapMode);
            }
        }

        #endregion

        /// <summary>
        /// Handles the internal save of the current Text and is called by Save and Save As
        /// </summary>
        /// <param name="filename">the filename</param>
        private void InternalSave(string filename)
        {
            var ioService = GetIoService();
            FileName = filename;
            isFilenameAvailable = true;

            var text = Ui.GetText(false);
            ioService.WriteTextFile(filename, text);

            RaisePropertyChange("FileName");
        }

        /// <summary>
        /// Handles the internal load of files and is called by ICommand delegates
        /// </summary>
        /// <param name="filename">the filename</param>
        private void InternalLoad(string filename)
        {
            var ioService = GetIoService();
            FileName = filename;
            isFilenameAvailable = true;

            var text = ioService.ReadTextFile(FileName);
            Ui.SetText(text);

            RaisePropertyChange("Text");
            RaisePropertyChange("FileName");
        }

        private void InternalNew()
        {
            FileName = "New";
            isFilenameAvailable = false;

            Text = String.Empty;
            Ui.SetText(String.Empty);

            RaisePropertyChange("Text");
            RaisePropertyChange("FileName");
        }

        private Settings GetSettings()
        {
            Settings? settings = null;

            var facade = FacadeFactory.Create();
            if (facade != null)
            {
                settings = facade.Get<Settings>(Bootstrap.SettingsId);
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            return settings;
        }
    }
}

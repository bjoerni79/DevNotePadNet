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
        private IMainViewUi Ui { get; set; }

        public MainViewModel()
        {
            Ui = null;

            Text = "123\n456";
            FileName = "lala.txt";

            // File
            New = new DefaultCommand(OnNew);
            Open = new DefaultCommand(OnOpen);
            Save = new DefaultCommand(OnSave);
            SaveAs = new DefaultCommand(OnSaveAs);

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

        public IRefreshCommand ToggleScrollbar { get; set; }
        public IRefreshCommand ToggleLineWrap { get; set; }

        public IRefreshCommand JsonFormatter { get; set; }

        public IRefreshCommand About { get; set; }

        #endregion

        public string Text { get; set; }

        public string FileName { get; set; }

        public bool LineWrapMode { get; set; }

        public bool ScrollbarMode { get; set; }

        private void OnNew()
        {

        }

        private void OnOpen()
        {
            var dialogService = GetDialogService();
            var fileName = dialogService.ShowOpenFileNameDialog("Open New File", "*.txt", String.Empty);

            if (fileName != null)
            {
                var ioService = GetIoService();
                FileName = fileName;

                var text = ioService.ReadTextFile(FileName);
                Text = text;

                RaisePropertyChange("Text");
                RaisePropertyChange("FileName");
            }
        }

        private void OnSave()
        {

        }

        private void OnSaveAs()
        {

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

        private Settings GetSettings()
        {
            Settings settings = null;

            var facade = FacadeFactory.Create();
            if (facade != null)
            {
                settings = facade.Get<Settings>(Bootstrap.SettingsId);
            }

            if (settings == null)
            {
                throw new ArgumentNullException("Settings object is not available in the IoC container!");
            }

            return settings;
        }
    }
}

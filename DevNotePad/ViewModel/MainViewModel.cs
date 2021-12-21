using DevNotePad.Feature;
using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared;
using Generic.MVVM;
using Generic.MVVM.IOC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class MainViewModel : AbstractViewModel, IMainViewModel
    {
        private IMainViewUi? Ui { get; set; }

        private EditorState currentState;

        private string initialText;
        private DateTime latestTimeStamp;

        public MainViewModel()
        {
            // File
            New = new DefaultCommand(OnNew);
            Open = new DefaultCommand(OnOpen);
            Save = new DefaultCommand(OnSave);
            SaveAs = new DefaultCommand(OnSaveAs);
            Reload = new DefaultCommand(OnReload);
            Close = new DefaultCommand(OnClose);

            //Tools
            JsonFormatter = new DefaultCommand(OnJsonFormatter);
            JsonToStringParser = new DefaultCommand(OnJsonToString);
            JsonToTreeParser = new DefaultCommand(OnJsonToTree);

            // Layout
            ToggleLineWrap = new DefaultCommand(OnToggleTextWrap);
            ToggleScrollbar = new DefaultCommand(OnToggleScrollbar);

            // About
            About = new DefaultCommand(OnAbout);

            FileName = "Not Defined";
            initialText = String.Empty;
        }

        #region Commands

        public IRefreshCommand New { get; set; }

        public IRefreshCommand Open { get; set; }

        public IRefreshCommand Save { get; set; }

        public IRefreshCommand SaveAs { get; set; }

        public IRefreshCommand Reload { get; set; }

        public IRefreshCommand Close { get; set; }

        public IRefreshCommand ToggleScrollbar { get; set; }
        public IRefreshCommand ToggleLineWrap { get; set; }

        public IRefreshCommand JsonFormatter { get; set; }

        public IRefreshCommand JsonToStringParser { get; set; }

        public IRefreshCommand JsonToTreeParser { get; set; }

        public IRefreshCommand About { get; set; }

        #endregion


        #region Command Delegates

        private void OnNew()
        {
            InternalNew();
        }

        private void OnReload()
        {
            InternalReload();
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
            if (currentState == EditorState.Loaded || currentState == EditorState.Saved)
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

        private void OnClose()
        {
            if (currentState == EditorState.Changed || currentState == EditorState.ChangedNew)
            {
                //TODO: Ask for saving after the notifier bug is fixed!
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
            if (Ui == null)
            {
                ShowError(new ApplicationException("Please init Ui first"), "Application");
            }
            else
            {
                var isTextSelected = Ui.IsTextSelected();
                var input = Ui.GetText(isTextSelected);
                try
                {
                    IJsonComponent jsonComponent = new JsonComponent();
                    var result = jsonComponent.Formatter(input);

                    Ui.SetText(result, isTextSelected);
                }
                catch (FeatureException featureException)
                {
                    ShowError(featureException, "JSON");
                }
            }
        }

        private void OnJsonToString()
        {
            if (Ui == null)
            {
                ShowError(new ApplicationException("Please init Ui first"), "Application");
            }
            else
            {
                var isTextSelected = Ui.IsTextSelected();
                var input = Ui.GetText(isTextSelected);

                try
                {
                    IJsonComponent jsonComponent = new JsonComponent();
                    var la = jsonComponent.ParseToString(input);

                    Ui.AddToScratchPad(la);
                }
                catch (FeatureException featureException)
                {
                    ShowError(featureException, "JSON");
                }
            }
        }

        private void OnJsonToTree()
        {
            if (Ui == null)
            {
                ShowError(new ApplicationException("Please init Ui first"), "Application");
            }
            else
            {
                var isTextSelected = Ui.IsTextSelected();
                var input = Ui.GetText(isTextSelected);

                try
                {
                    IJsonComponent jsonComponent = new JsonComponent();
                    var rootNode = jsonComponent.ParseToTree(input);

                    Nodes = new ObservableCollection<ItemNode>();
                    Nodes.Add(rootNode);
                    RaisePropertyChange("Nodes");
                }
                catch (FeatureException featureException)
                {
                    ShowError(featureException, "JSON");
                }
            }
        }

        private void OnAbout()
        {
            if (Ui != null)
            {
                Ui.ShowAbout();
            }
        }

        #endregion

        public ObservableCollection<ItemNode>? Nodes { get; set; }

        public string FileName { get; set; }

        public bool LineWrapMode { get; set; }

        public bool ScrollbarMode { get; set; }


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

        public void NotifyContentChanged(int added, int offset, int removed)
        {
            //TODO: This event fires every time. Add a filter logic for only triggering the changes by the user or formatter

            if (currentState == EditorState.New)
            {
                currentState = EditorState.ChangedNew;
            }
            else
            {
                currentState = EditorState.Changed;
            }
        }

        public bool IsChanged()
        {
            var isChangeRequired = currentState == EditorState.Changed || currentState == EditorState.ChangedNew;
            return isChangeRequired;
        }

        #endregion

        /// <summary>
        /// Handles the internal save of the current Text and is called by Save and Save As
        /// </summary>
        /// <param name="filename">the filename</param>
        private void InternalSave(string filename)
        {
            try
            {
                var ioService = GetIoService();
                FileName = filename;
                currentState = EditorState.Saved;
                latestTimeStamp = DateTime.Now;

                initialText = Ui!.GetText(false);
                ioService.WriteTextFile(filename, initialText);

                RaisePropertyChange("FileName");
            }
            catch (Exception ex)
            {
                ShowError(ex, "Save File");
            }

        }

        /// <summary>
        /// Handles the internal load of files and is called by ICommand delegates
        /// </summary>
        /// <param name="filename">the filename</param>
        private void InternalLoad(string filename)
        {
            try
            {
                var ioService = GetIoService();
                FileName = filename;
                currentState = EditorState.Loaded;

                //TODO: Store the timestamp of the file right now
                latestTimeStamp = ioService.GetModificationTimeStamp(filename);

                initialText = ioService.ReadTextFile(FileName);
                Ui!.SetText(initialText);

                RaisePropertyChange("Text");
                RaisePropertyChange("FileName");
            }
            catch (Exception ex)
            {
                ShowError(ex, "Load File");
            }

        }

        /// <summary>
        /// Handles the creation of new files
        /// </summary>
        private void InternalNew()
        {
            bool proceed = true;

            if (currentState == EditorState.ChangedNew || currentState == EditorState.Changed)
            {
                var dialogService = GetDialogService();
                proceed = dialogService.ShowConfirmationDialog("The text is not saved yet. Do you want to continue?","New");
            }

            if (proceed)
            {
                FileName = "New";
                currentState = EditorState.New;
                initialText = String.Empty;
                latestTimeStamp = DateTime.Now;

                Ui!.SetText(initialText);

                RaisePropertyChange("Text");
                RaisePropertyChange("FileName");
            }
        }

        /// <summary>
        /// Handles the reload of a file
        /// </summary>
        private void InternalReload()
        {
            //TODO: If state is new, there is nothing to reload...
            if (currentState == EditorState.New)
            {
                // TODO: Notify via toolbar that none action is taken..
            }
            else
            {
                bool checkForReload = true;

                //TODO:  Saving first? Check the states...
                if (currentState == EditorState.Changed || currentState == EditorState.ChangedNew)
                {
                    var dialogService = GetDialogService();
                    checkForReload = dialogService.ShowConfirmationDialog("The text is not saved yet. Do you want to reload?", "Reload");
                }

                if (checkForReload)
                {
                    try
                    {
                        //TODO: Any file changes?  What is the creation date? etc
                        var io = GetIoService();
                        var currentModifiedTimestamp = io.GetModificationTimeStamp(FileName);

                        if (currentModifiedTimestamp > latestTimeStamp)
                        {
                            InternalLoad(FileName);
                        }

                    }
                    catch (Exception ex)
                    {
                        ShowError(ex, "Reload");
                    }


                }
                else
                {
                    //TODO Notify that no reload is required
                }

            }
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

using DevNotePad.Features;
using DevNotePad.Features.Json;
using DevNotePad.Features.Shared;
using DevNotePad.Features.Xml;
using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared;
using DevNotePad.Shared.Event;
using Generic.MVVM;
using Generic.MVVM.IOC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevNotePad.ViewModel
{
    public class MainViewModel : AbstractViewModel, IMainViewModel
    {
        private readonly string ApplicationComponent = "Application";
        private readonly string JsonComponent = "JSON";
        private readonly string XmlComponent = "XML";
        private readonly string TextComponent = "Text";

        private IMainViewUi? Ui { get; set; }

        private EditorState currentState;

        private string initialText;
        private DateTime latestTimeStamp;
        private string fileName;

        public bool LineWrapMode { get; private set; }

        public ObservableCollection<ItemNode>? Nodes { get; set; }

        public string State { get; private set; }

        public MainViewModel()
        {
            InitMenu();

            fileName = "Not Defined";
            initialText = String.Empty;
            State = String.Empty;
        }

        #region Commands

        // File

        public IRefreshCommand? New { get; set; }

        public IRefreshCommand? Open { get; set; }

        public IRefreshCommand? Save { get; set; }

        public IRefreshCommand? SaveAs { get; set; }

        public IRefreshCommand? Reload { get; set; }

        public IRefreshCommand? Close { get; set; }

        // Edit

        public IRefreshCommand? Find { get; set; }

        public IRefreshCommand? Replace { get; set; }
        public IRefreshCommand? CopyToScratchPad { get; set; }

        public IRefreshCommand? Cut { get; set; }

        public IRefreshCommand? Copy { get; set; }

        public IRefreshCommand? Paste { get; set; }

        public IRefreshCommand? SelectAll { get; set; }

        // Layout

        public IRefreshCommand? ToggleLineWrap { get; set; }



        // Tools

        public IRefreshCommand? JsonFormatter { get; set; }

        public IRefreshCommand? JsonToStringParser { get; set; }

        public IRefreshCommand? JsonToTreeParser { get; set; }

        public IRefreshCommand? XmlFormatter { get; set; }

        public IRefreshCommand? XmlToStringParser { get; set; }

        public IRefreshCommand? XmlToTreeParser { get; set; }

        public IRefreshCommand? TextSplit { get; set; }

        // ScratchPad

        public IRefreshCommand? ScratchPadClearAll { get; set; }

        public IRefreshCommand? ScratchPadClearText { get; set; }

        public IRefreshCommand? ScratchPadClearTree { get; set; }

        public IRefreshCommand? ScratchPadCopyClipboard { get; set; }

        // About

        public IRefreshCommand? About { get; set; }

        public IRefreshCommand? Refresh { get; set; }

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
            if (currentState == EditorState.New || currentState == EditorState.ChangedNew)
            {
                OnSaveAs();
            }

            // Just save it
            InternalSave(fileName);
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
            Ui!.CloseByViewModel();
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
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var isTextSelected = Ui!.IsTextSelected();
                var input = Ui.GetText(isTextSelected);
                try
                {
                    IJsonComponent jsonComponent = FeatureFactory.CreateJson();
                    var result = jsonComponent.Formatter(input);

                    Ui.SetText(result, isTextSelected);

                    TriggerToolbarNotification(new UpdateStatusBarParameter("JSON file is formatted", false));
                }
                catch (FeatureException featureException)
                {
                    ShowError(featureException, JsonComponent);
                    TriggerToolbarNotification(new UpdateStatusBarParameter("JSON operation failed", true));
                }
            }
        }

        private void OnJsonToString()
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var isTextSelected = Ui!.IsTextSelected();
                var input = Ui.GetText(isTextSelected);

                try
                {
                    IJsonComponent jsonComponent = FeatureFactory.CreateJson();
                    var la = jsonComponent.ParseToString(input);

                    Ui.AddToScratchPad(la);
                    Ui.FocusScratchPad();

                    TriggerToolbarNotification(new UpdateStatusBarParameter("JSON content rendered to ScratchPad", false));
                }
                catch (FeatureException featureException)
                {
                    ShowError(featureException, JsonComponent);
                    TriggerToolbarNotification(new UpdateStatusBarParameter("JSON operation failed", true));
                }
            }
        }

        private void OnJsonToTree()
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var isTextSelected = Ui!.IsTextSelected();
                var input = Ui.GetText(isTextSelected);

                try
                {
                    IJsonComponent jsonComponent = FeatureFactory.CreateJson();
                    var rootNode = jsonComponent.ParseToTree(input);

                    Nodes = new ObservableCollection<ItemNode>();
                    Nodes.Add(rootNode);
                    RaisePropertyChange("Nodes");

                    Ui.FocusTree();
                    TriggerToolbarNotification(new UpdateStatusBarParameter("JSON content rendered to tree", false));
                }
                catch (FeatureException featureException)
                {
                    ShowError(featureException, JsonComponent);
                    TriggerToolbarNotification(new UpdateStatusBarParameter("JSON operation failed", true));
                }
            }
        }

        private void OnXmlFormatter()
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var isTextSelected = Ui!.IsTextSelected();
                var input = Ui.GetText(isTextSelected);

                try
                {
                    IXmlComponent component = FeatureFactory.CreateXml();
                    var formatted = component.Formatter(input);

                    Ui.SetText(formatted, isTextSelected);
                    TriggerToolbarNotification(new UpdateStatusBarParameter("XML file formatted", false));
                }
                catch (FeatureException featureException)
                {
                    ShowError(featureException, XmlComponent);
                    TriggerToolbarNotification(new UpdateStatusBarParameter("XML operation failed", true));
                }
            }
        }

        private void OnXmlToString()
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var isTextSelected = Ui!.IsTextSelected();
                var input = Ui.GetText(isTextSelected);

                try
                {
                    IXmlComponent jsonComponent = FeatureFactory.CreateXml();
                    var la = jsonComponent.ParseToString(input);

                    Ui.AddToScratchPad(la);
                    Ui.FocusScratchPad();

                    TriggerToolbarNotification(new UpdateStatusBarParameter("XML content rendered to ScratchPad", false));
                }
                catch (FeatureException featureException)
                {
                    ShowError(featureException, JsonComponent);
                    TriggerToolbarNotification(new UpdateStatusBarParameter("XML operation failed", true));
                }
            }
        }

        private void OnXmlToTree()
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var isTextSelected = Ui!.IsTextSelected();
                var input = Ui.GetText(isTextSelected);

                try
                {
                    //TODO
                    IXmlComponent component = FeatureFactory.CreateXml();
                    var rootNode = component.ParseToTree(input);

                    Nodes = new ObservableCollection<ItemNode>();
                    Nodes.Add(rootNode);
                    RaisePropertyChange("Nodes");

                    Ui.FocusTree();
                }
                catch (FeatureException featureException)
                {
                    ShowError(featureException, XmlComponent);
                    TriggerToolbarNotification(new UpdateStatusBarParameter("XML operation failed", true));
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

        private bool CheckForUi()
        {
            if (Ui == null)
            {
                ShowError(new ApplicationException("Please init Ui first"), ApplicationComponent);
                return false;
            }

            return true;
            
        }

        private void OnClearAllScratchPad()
        {
            OnClearTextScratchPad();
            OnClearTreeScratchPad();

            TriggerToolbarNotification(new UpdateStatusBarParameter("ScratchPad and Tree are empty", false));
        }

        private void OnClearTextScratchPad()
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                Ui!.CleanUpScratchPad();

                TriggerToolbarNotification(new UpdateStatusBarParameter("ScratchPad is empty", false));
            }
        }

        private void OnClearTreeScratchPad()
        {
            Nodes = new ObservableCollection<ItemNode>();
            RaisePropertyChange("Nodes");

            TriggerToolbarNotification(new UpdateStatusBarParameter("ScratchPad Tree is empty", false));
        }

        private void OnCopyClipboardToScratchPad()
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var containsText = Clipboard.ContainsText();
                if (containsText)
                {
                    var content = Clipboard.GetText();
                    Ui!.AddToScratchPad(content);
                }
            }
        }

        private void OnFind()
        {
            var dialogService = GetDialogService();
            dialogService.OpenFindDialog(Ui);
        }

        private void OnReplace()
        {
            var dialogService = GetDialogService();
            dialogService.OpenReplaceDialog(Ui);
        }

        private void OnCopyToScratchPad()
        {
            var isSelected = Ui!.IsTextSelected();
            var text = Ui.GetText(isSelected);

            Ui.AddToScratchPad(text);
        }

        private void OnRefresh()
        {
            //TODO
        }

        private void OnTextSplit()
        {
            var isSelected = Ui!.IsTextSelected();
            if (!isSelected)
            {
                TriggerToolbarNotification(new UpdateStatusBarParameter("No Text selected. Please select a text first", true));
            }
            else
            {
                try
                {
                    var formatter = new TextFormatter();

                    //TODO: Write some smart code here...
                }
                catch (Exception ex)
                {
                    ShowError(ex, TextComponent);
                }
            }
        }

        #endregion

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
                //ScrollbarMode = settings.ScrollbarAlwaysOn;
                LineWrapMode = settings.LineWrap;

                //Ui.SetScrollbars(ScrollbarMode);
                Ui.SetWordWrap(LineWrapMode);
            }

            TriggerToolbarNotification(new UpdateStatusBarParameter("Ready", false));
        }

        public void NotifyContentChanged(int added, int offset, int removed)
        {
            int internalTextLength = initialText.Length;
            var loadedEvent = offset == 0 && internalTextLength == added;

            var isChangedSelected = currentState == EditorState.Changed || currentState == EditorState.ChangedNew;
            if (!loadedEvent && !isChangedSelected)
            {
                if (currentState == EditorState.New)
                {
                    currentState = EditorState.ChangedNew;
                }
                else
                {
                    currentState = EditorState.Changed;
                }

                State = "Changed";
                RaisePropertyChange("State");
            }
        }

        public bool IsChanged()
        {
            var isChangeRequired = currentState == EditorState.Changed || currentState == EditorState.ChangedNew;
            return isChangeRequired;
        }

        #endregion

        #region Internal Logic

        private void PerfromClipboardAction(ClipboardActionEnum action)
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                Ui!.PerformClipboardAction(action);
            }
        }

        /// <summary>
        /// Handles the internal save of the current Text and is called by Save and Save As
        /// </summary>
        /// <param name="filename">the filename</param>
        private void InternalSave(string targetfilename)
        {
            try
            {
                var ioService = GetIoService();


                //TODO: Check for update! What happens if the file is newer than the latest load?



                initialText = Ui!.GetText(false);
                ioService.WriteTextFile(targetfilename, initialText);
                currentState = EditorState.Saved;
                latestTimeStamp = DateTime.Now;


                fileName = targetfilename;
                Ui.SetFilename(fileName);

                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Content is saved", false));

                State = "Saved";
                RaisePropertyChange("State");
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
        private void InternalLoad(string sourceFilename)
        {
            try
            {
                var ioService = GetIoService();
                fileName = sourceFilename; ;
                currentState = EditorState.Loaded;

                //TODO: Store the timestamp of the file right now
                latestTimeStamp = ioService.GetModificationTimeStamp(fileName);

                initialText = ioService.ReadTextFile(fileName);
                Ui!.SetText(initialText);
                Ui.SetFilename(fileName);

                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("File is loaded", false));

                State = "Loaded";
                RaisePropertyChange("State");
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
                proceed = dialogService.ShowConfirmationDialog("The text is not saved yet. Do you want to continue?","New","Create New");
            }

            if (proceed)
            {
                fileName = "New";
                currentState = EditorState.New;
                initialText = String.Empty;
                latestTimeStamp = DateTime.Now;

                Ui!.SetText(initialText);

                RaisePropertyChange("Text");
                RaisePropertyChange("FileName");
                Ui.SetFilename(fileName);

                TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("New file created", false));

                State = "New";
                RaisePropertyChange("State");
            }
        }

        /// <summary>
        /// Handles the reload of a file
        /// </summary>
        private void InternalReload()
        {
            if (currentState == EditorState.New)
            {
                TriggerToolbarNotification(new UpdateStatusBarParameter("Please save the file first", false));
            }
            else
            {
                try
                {
                    // Any file changes?  What is the creation date? etc
                    var io = GetIoService();
                    var currentModifiedTimestamp = io.GetModificationTimeStamp(fileName);

                    if (currentModifiedTimestamp > latestTimeStamp)
                    {
                        bool doReload = true;
                        if (currentState == EditorState.Changed || currentState == EditorState.ChangedNew)
                        {
                            var dialogService = GetDialogService();
                            doReload = dialogService.ShowConfirmationDialog("The text is not saved yet. Do you want to reload?", "Reload","Reload Content");
                        }


                        if (doReload)
                        {
                            InternalLoad(fileName);
                        }  
                        else
                        {
                            TriggerToolbarNotification(new UpdateStatusBarParameter("Reload cancelled", true));
                        }
                    }
                    else
                    {
                        TriggerToolbarNotification(new UpdateStatusBarParameter("Reload not required", false));
                    }

                }
                catch (Exception ex)
                {
                    ShowError(ex, "Reload");
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

        private void InitMenu()
        {
            // File
            New = new DefaultCommand(OnNew);
            Open = new DefaultCommand(OnOpen);
            Save = new DefaultCommand(OnSave);
            SaveAs = new DefaultCommand(OnSaveAs);
            Reload = new DefaultCommand(OnReload);
            Close = new DefaultCommand(OnClose);

            // Edit
            Find = new DefaultCommand(OnFind);
            Replace = new DefaultCommand(OnReplace);
            CopyToScratchPad = new DefaultCommand(OnCopyToScratchPad);
            // Cut
            Cut = new DefaultCommand(() => PerfromClipboardAction(ClipboardActionEnum.Cut));
            // Copy
            Copy = new DefaultCommand(() => PerfromClipboardAction(ClipboardActionEnum.Copy));
            // Paste
            Paste = new DefaultCommand(() => PerfromClipboardAction(ClipboardActionEnum.Paste));
            // Select All
            SelectAll = new DefaultCommand(() => PerfromClipboardAction(ClipboardActionEnum.SelectAll));

            //Tools
            JsonFormatter = new DefaultCommand(OnJsonFormatter);
            JsonToStringParser = new DefaultCommand(OnJsonToString);
            JsonToTreeParser = new DefaultCommand(OnJsonToTree);
            XmlFormatter = new DefaultCommand(OnXmlFormatter);
            XmlToStringParser = new DefaultCommand(OnXmlToString);
            XmlToTreeParser = new DefaultCommand(OnXmlToTree);
            TextSplit = new DefaultCommand(OnTextSplit);

            // Layout
            ToggleLineWrap = new DefaultCommand(OnToggleTextWrap);
            Refresh = new DefaultCommand(OnRefresh);

            // ScratchPad
            ScratchPadClearAll = new DefaultCommand(OnClearAllScratchPad);
            ScratchPadClearText = new DefaultCommand(OnClearTextScratchPad);
            ScratchPadClearTree = new DefaultCommand(OnClearTreeScratchPad);
            ScratchPadCopyClipboard = new DefaultCommand(OnCopyClipboardToScratchPad);

            // About
            About = new DefaultCommand(OnAbout);
        }

        #endregion
    }
}

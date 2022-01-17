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

        private const string DefaultExtension = "All|*.*|Text Files|*.txt|Log Files|*.log|XML|*.xml|JSON|*.json";

        private IMainViewUi? Ui { get; set; }

        private ITextComponent? textComponent;

        private ITextComponent? scratchPadComponent;

        private IFileLogic? textLogic;

        private IFileLogic? scratchPadLogic;

        public bool LineWrapMode { get; private set; }

        public ObservableCollection<ItemNode>? Nodes { get; set; }

        public string State { get; private set; }

        public MainViewModel()
        {
            State = "Ready";
            InitMenu();
        }

        #region Commands

        // File

        public IRefreshCommand? New { get; private set; }

        public IRefreshCommand? Open { get; private set; }

        public IRefreshCommand? Save { get; private set; }

        public IRefreshCommand? SaveAs { get; private set; }

        public IRefreshCommand? Reload { get; private set; }

        public IRefreshCommand? Close { get; private set; }

        // Edit

        public IRefreshCommand? Find { get; private set; }

        public IRefreshCommand? Replace { get; private set; }


        public IRefreshCommand? CopyToText { get; private set; }

        public IRefreshCommand? Cut { get; private set; }



        public IRefreshCommand? Copy { get; private set; }

        public IRefreshCommand? Paste { get; private set; }


        public IRefreshCommand? SelectAll { get; private set; }

        // Layout

        public IRefreshCommand? ToggleLineWrap { get; private set; }



        // Tools

        public IRefreshCommand? JsonFormatter { get; private set; }

        public IRefreshCommand? JsonToStringParser { get; private set; }

        public IRefreshCommand? JsonToTreeParser { get; private set; }

        public IRefreshCommand? XmlFormatter { get; private set; }

        public IRefreshCommand? XmlToStringParser { get; private set; }

        public IRefreshCommand? XmlToTreeParser { get; private set; }

        public IRefreshCommand? TextSplit { get; private set; }

        public IRefreshCommand? TextGroup { get; private set; }

        public IRefreshCommand? TextToLower { get; private set; }

        public IRefreshCommand? TextToUpper { get; private set; }

        public IRefreshCommand? TextTrim { get; private set; }

        public IRefreshCommand? TextCountLength { get; private set; }

        // ScratchPad

        public IRefreshCommand? ScratchPadClearAll { get; private set; }

        public IRefreshCommand? ScratchPadClearText { get; private set; }

        public IRefreshCommand? ScratchPadClearTree { get; private set; }

        public IRefreshCommand? ScratchPadCopyClipboard { get; private set; }

        public IRefreshCommand? CopyToScratchPad { get; private set; }

        public IRefreshCommand? ScratchPadCut { get; private set; }

        public IRefreshCommand? ScratchPadPaste { get; private set; }

        public IRefreshCommand? ScratchPadCopy { get; private set; }

        public IRefreshCommand? ScratchPadJsonFormat { get; private set; }

        public IRefreshCommand? ScratchPadJsonToTree { get; private set; }

        public IRefreshCommand? ScratchPadXmlFormat { get; private set; }

        public IRefreshCommand? ScratchPadXmlToTree { get; private set; }

        public IRefreshCommand? ScratchPadSplit { get; private set; }

        public IRefreshCommand? ScratchPadGroup { get; private set; }

        public IRefreshCommand? ScratchpadToLower { get; private set; }

        public IRefreshCommand? ScratchPadToUpper { get; private set; } 

        public IRefreshCommand? ScratchPadTrim { get; private set; }

        public IRefreshCommand? ScratchPadCountLength { get; private set; }

        // About

        public IRefreshCommand? About { get; private set; }

        public IRefreshCommand? Refresh { get; private set; }

        #endregion

        #region Command Delegates

        private void OnFileOperation(FileOperation operation)
        {
            if (textLogic != null)
            {
                var dialogService = ServiceHelper.GetDialogService();

                // File New
                if (operation == FileOperation.New)
                {
                    textLogic.New();
                }

                // File Reload
                if (operation == FileOperation.Reload)
                {
                    textLogic.Reload();
                }

                // File Open
                if (operation == FileOperation.Open)
                {
                    var result = dialogService.ShowOpenFileNameDialog(DefaultExtension);
                    if (result.IsConfirmed)
                    {
                        textLogic.Load(result.File);
                    }
                }

                // File Save
                if (operation == FileOperation.Save)
                {
                    if (textLogic.CurrentState == EditorState.New || textLogic.CurrentState == EditorState.ChangedNew)
                    {
                        var result = dialogService.ShowSaveFileDialog(DefaultExtension);
                        if (result.IsConfirmed)
                        {
                            textLogic.Save(result.File);
                        }
                    }
                    else
                    {
                        // Just save it
                        textLogic.Save(textLogic.FileName);
                    }
                }

                // File Save As
                if (operation == FileOperation.SaveAs)
                {
                    var result = dialogService.ShowSaveFileDialog(DefaultExtension);
                    if (result.IsConfirmed)
                    {
                        textLogic.Save(result.File);
                    }
                }

                UpdateFileStatus();
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

        private void OnJson(JsonOperation operation)
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var isTextSelected = textComponent!.IsTextSelected();
                var input = textComponent.GetText(isTextSelected);

                try
                {
                    IJsonComponent jsonComponent = FeatureFactory.CreateJson();

                    // JSON to Tree action
                    if (operation == JsonOperation.ToTree)
                    {
                        var rootNode = jsonComponent.ParseToTree(input);

                        Nodes = new ObservableCollection<ItemNode>();
                        Nodes.Add(rootNode);
                        RaisePropertyChange("Nodes");

                        Ui.FocusTree();
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON content rendered to tree", false));
                    }

                    // JSON to text action
                    if (operation == JsonOperation.ToText)
                    {
                        var la = jsonComponent.ParseToString(input);

                        scratchPadComponent.AddText(la);
                        Ui.FocusScratchPad();
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON content rendered to ScratchPad", false));
                    }

                    // JSON format action
                    if (operation == JsonOperation.Format)
                    {
                        var result = jsonComponent.Formatter(input);

                        textComponent.SetText(result, isTextSelected);
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON file is formatted", false));
                    }

                }
                catch (FeatureException featureException)
                {
                    ServiceHelper.ShowError(featureException, JsonComponent);
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON operation failed", true));
                }
            }
        }

        private void OnScratchPadXml(XmlOperation operation)
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var isTextSelected = scratchPadComponent!.IsTextSelected();
                var input = scratchPadComponent.GetText(isTextSelected);

                try
                {
                    IXmlComponent component = FeatureFactory.CreateXml();

                    if (operation == XmlOperation.Format)
                    {
                        var formatted = component.Formatter(input);

                        scratchPadComponent.SetText(formatted, isTextSelected);
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML file formatted", false));
                    }

                    if (operation == XmlOperation.ToTree)
                    {
                        var rootNode = component.ParseToTree(input);

                        Nodes = new ObservableCollection<ItemNode>();
                        Nodes.Add(rootNode);
                        RaisePropertyChange("Nodes");

                        Ui.FocusTree();
                    }
                }
                catch (FeatureException featureException)
                {
                    ServiceHelper.ShowError(featureException, XmlComponent);
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML operation failed", true));
                }
            }
        }
        private void OnScratchPadJson(JsonOperation operation)
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var isTextSelected = scratchPadComponent!.IsTextSelected();
                var input = scratchPadComponent.GetText(isTextSelected);

                try
                {
                    IJsonComponent jsonComponent = FeatureFactory.CreateJson();

                    if (operation == JsonOperation.Format)
                    {
                        var result = jsonComponent.Formatter(input);

                        scratchPadComponent.SetText(result, isTextSelected);
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON file is formatted", false));
                    }

                    if (operation == JsonOperation.ToTree)
                    {
                        var rootNode = jsonComponent.ParseToTree(input);

                        Nodes = new ObservableCollection<ItemNode>();
                        Nodes.Add(rootNode);
                        RaisePropertyChange("Nodes");

                        Ui.FocusTree();
                    }
                }
                catch (FeatureException featureException)
                {
                    ServiceHelper.ShowError(featureException, JsonComponent);
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON operation failed", true));
                }
            }
        }

        private void OnXml(XmlOperation operation)
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                var isTextSelected = textComponent!.IsTextSelected();
                var input = textComponent.GetText(isTextSelected);

                try
                {
                    IXmlComponent component = FeatureFactory.CreateXml();

                    // Format Action
                    if (operation == XmlOperation.Format)
                    {
                        var formatted = component.Formatter(input);

                        textComponent.SetText(formatted, isTextSelected);
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML file formatted", false));
                    }

                    // To Text Action
                    if (operation == XmlOperation.ToText)
                    {
                        var la = component.ParseToString(input);

                        scratchPadComponent.AddText(la);
                        Ui.FocusScratchPad();

                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML content rendered to ScratchPad", false));
                    }

                    // To Tree Action
                    if (operation == XmlOperation.ToTree)
                    {
                        var rootNode = component.ParseToTree(input);

                        Nodes = new ObservableCollection<ItemNode>();
                        Nodes.Add(rootNode);
                        RaisePropertyChange("Nodes");

                        Ui.FocusTree();
                    }
                }
                catch (FeatureException featureException)
                {
                    ServiceHelper.ShowError(featureException, XmlComponent);
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML operation failed", true));
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
                ServiceHelper.ShowError(new ApplicationException("Please init Ui first"), ApplicationComponent);
                return false;
            }

            return true;
            
        }

        private void OnClearAllScratchPad()
        {
            OnClearTextScratchPad();
            OnClearTreeScratchPad();

            ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("ScratchPad and Tree are empty", false));
        }

        private void OnClearTextScratchPad()
        {
            bool isUiFound = CheckForUi();
            if (isUiFound)
            {
                Ui!.CleanUpScratchPad();

                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("ScratchPad is empty", false));
            }
        }

        private void OnClearTreeScratchPad()
        {
            Nodes = new ObservableCollection<ItemNode>();
            RaisePropertyChange("Nodes");

            ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("ScratchPad Tree is empty", false));
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
                    scratchPadComponent.AddText(content);
                }
            }
        }

        private void OnFind()
        {
            var dialogService = ServiceHelper.GetDialogService();
            dialogService.OpenFindDialog(Ui,textComponent);
        }

        private void OnReplace()
        {
            var dialogService = ServiceHelper.GetDialogService();
            dialogService.OpenReplaceDialog(Ui,textComponent);
        }

        private void OnCopyToScratchPad()
        {
            var isSelected = textComponent!.IsTextSelected();
            var text = textComponent.GetText(isSelected);

            scratchPadComponent.AddText(text);
        }

        private void OnCopyToText()
        {
            var isSelected = scratchPadComponent!.IsTextSelected();
            var text = scratchPadComponent.GetText(isSelected);

            textComponent.AddText(text);

        }

        private void OnText(IFileLogic logic,TextActionEnum textAction)
        {
            if (logic != null)
            {
                logic.Modify(textAction);
            }
        }

        private void OnTextClipboard(IFileLogic logic,ClipboardActionEnum action)
        {
            if (logic != null)
            {
                logic.PerfromClipboardAction(action);
            }
        }

        #endregion

        #region IMainViewModel

        public void Init(IMainViewUi ui, ITextComponent text, ITextComponent scratchPad)
        {
            Ui = ui;
            textComponent = text;
            scratchPadComponent = scratchPad;

            if (scratchPad != null)
            {
                scratchPadLogic = new InternalFileLogic(Ui, scratchPad);
            }

            textLogic = new InternalFileLogic(Ui,textComponent);
            textLogic.New();
            UpdateFileStatus();
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

            ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Ready", false));
        }

        public void NotifyContentChanged(int added, int offset, int removed)
        {
            if (textLogic != null)
            {
                int internalTextLength = textLogic.InitialText.Length;
                var loadedEvent = offset == 0 && internalTextLength == added;

                var isChangedSelected = textLogic.CurrentState == EditorState.Changed || textLogic.CurrentState == EditorState.ChangedNew;
                if (!loadedEvent && !isChangedSelected)
                {
                    if (textLogic.CurrentState == EditorState.New)
                    {
                        textLogic.CurrentState = EditorState.ChangedNew;
                    }
                    else
                    {
                        textLogic.CurrentState = EditorState.Changed;
                    }

                    UpdateFileStatus();
                }
            }
        }

        public bool IsChanged()
        {
            if (textLogic != null)
            {
                var isChangeRequired = textLogic.CurrentState == EditorState.Changed || textLogic.CurrentState == EditorState.ChangedNew;
                return isChangeRequired;
            }

            return false;

        }

        #endregion

        /// <summary>
        /// Updates the UI with the current state in the file logic
        /// </summary>
        private void UpdateFileStatus()
        {
            var currentUiState = "Unknown";
            if (textLogic != null)
            {
                var currentState = textLogic.CurrentState;
                if (currentState == EditorState.Changed || currentState == EditorState.ChangedNew)
                {
                    currentUiState = "Changed";
                }

                if (currentState == EditorState.New)
                {
                    currentUiState = "New";
                }

                if (currentState == EditorState.Saved)
                {
                    currentUiState = "Saved";
                }

                if (currentState == EditorState.Loaded)
                {
                    currentUiState = "Loaded";
                }
            }

            // Notify the UI
            State = currentUiState;
            RaisePropertyChange("State");
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
            New = new DefaultCommand(()=>OnFileOperation(FileOperation.New));
            Open = new DefaultCommand(()=>OnFileOperation(FileOperation.Open));
            Save = new DefaultCommand(()=>OnFileOperation(FileOperation.Save));
            SaveAs = new DefaultCommand(()=>OnFileOperation(FileOperation.SaveAs));
            Reload = new DefaultCommand(()=>OnFileOperation(FileOperation.Reload));
            Close = new DefaultCommand(OnClose);

            // Edit
            Find = new DefaultCommand(OnFind);
            Replace = new DefaultCommand(OnReplace);
            CopyToScratchPad = new DefaultCommand(OnCopyToScratchPad);
            Cut = new DefaultCommand(() => OnTextClipboard(textLogic,ClipboardActionEnum.Cut));
            Copy = new DefaultCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.Copy));
            Paste = new DefaultCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.Paste));
            SelectAll = new DefaultCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.SelectAll));

            //Tools
            JsonFormatter = new DefaultCommand(()=>OnJson(JsonOperation.Format));
            JsonToStringParser = new DefaultCommand(()=>OnJson(JsonOperation.ToText));
            JsonToTreeParser = new DefaultCommand(()=>OnJson(JsonOperation.ToTree));
            XmlFormatter = new DefaultCommand(()=>OnXml(XmlOperation.Format));
            XmlToStringParser = new DefaultCommand(()=>OnXml(XmlOperation.ToText));
            XmlToTreeParser = new DefaultCommand(()=>OnXml(XmlOperation.ToTree));
            TextSplit = new DefaultCommand(()=>OnText(textLogic,TextActionEnum.Split));
            TextGroup = new DefaultCommand(() => OnText(textLogic, TextActionEnum.Group));
            TextToLower = new DefaultCommand(() => OnText(textLogic, TextActionEnum.ToLower));
            TextToUpper = new DefaultCommand(() => OnText(textLogic, TextActionEnum.ToUpper));
            TextTrim = new DefaultCommand(() => OnText(textLogic, TextActionEnum.Trim));
            TextCountLength = new DefaultCommand(() => OnText(textLogic, TextActionEnum.LengthCount));

            // Layout
            ToggleLineWrap = new DefaultCommand(OnToggleTextWrap);

            // ScratchPad
            CopyToText = new DefaultCommand(OnCopyToText);
            ScratchPadClearAll = new DefaultCommand(OnClearAllScratchPad);
            ScratchPadClearText = new DefaultCommand(OnClearTextScratchPad);
            ScratchPadClearTree = new DefaultCommand(OnClearTreeScratchPad);
            ScratchPadCopyClipboard = new DefaultCommand(OnCopyClipboardToScratchPad);
            ScratchPadCopy = new DefaultCommand(() => OnTextClipboard(scratchPadLogic, ClipboardActionEnum.Copy));
            ScratchPadCut = new DefaultCommand(() => OnTextClipboard(scratchPadLogic, ClipboardActionEnum.Cut));
            ScratchPadPaste = new DefaultCommand(() => OnTextClipboard(scratchPadLogic, ClipboardActionEnum.Paste));
            ScratchPadXmlFormat = new DefaultCommand(()=>OnScratchPadXml(XmlOperation.Format));
            ScratchPadXmlToTree = new DefaultCommand(() => OnScratchPadXml(XmlOperation.ToTree));
            ScratchPadJsonFormat = new DefaultCommand(() => OnScratchPadJson(JsonOperation.Format));
            ScratchPadJsonToTree = new DefaultCommand(() => OnScratchPadJson(JsonOperation.ToTree));

            //TODO: Text Actions
            ScratchPadSplit = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.Split));
            ScratchPadGroup = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.Group));
            ScratchpadToLower = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.ToLower));
            ScratchPadToUpper = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.ToUpper));
            ScratchPadTrim = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.Trim));
            ScratchPadCountLength = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.LengthCount));

            // About
            About = new DefaultCommand(OnAbout);
        }

        /// <summary>
        /// JSON Action commands 
        /// </summary>
        private enum JsonOperation
        {
            Format,
            ToTree,
            ToText
        }

        /// <summary>
        /// XML Action commands
        /// </summary>
        private enum XmlOperation
        {
            Format,
            ToTree,
            ToText
        }

        /// <summary>
        /// File Action commands
        /// </summary>
        private enum FileOperation
        {
            New,
            Open,
            Save,
            SaveAs,
            Reload
        }
    }
}

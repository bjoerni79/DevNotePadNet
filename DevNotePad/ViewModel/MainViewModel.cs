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

        private const string DefaultExtension = "All|*.*|Text Files|*.txt|Log Files|*.log|XML|*.xml|JSON|*.json";

        private IMainViewUi? Ui { get; set; }

        private IFileLogic? fileLogic;

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
        public IRefreshCommand? CopyToScratchPad { get; private set; }

        public IRefreshCommand? CopyToText { get; private set; }

        public IRefreshCommand? Cut { get; private set; }

        public IRefreshCommand? ScratchPadCut { get; private set; }

        public IRefreshCommand? Copy { get; private set; }

        public IRefreshCommand? ScratchPadCopy { get; private set; }

        public IRefreshCommand? Paste { get; private set; }

        public IRefreshCommand? ScratchPadPaste { get; private set; }

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

        // About

        public IRefreshCommand? About { get; private set; }

        public IRefreshCommand? Refresh { get; private set; }

        #endregion

        #region Command Delegates

        private void OnNew()
        {
            if (fileLogic != null)
            {
                fileLogic.InternalNew();
                UpdateFileStatus();
            }
        }

        private void OnReload()
        {
            if (fileLogic != null)
            {
                fileLogic.InternalReload();
                UpdateFileStatus();
            }
            
        }

        private void OnOpen()
        {
            if (fileLogic != null)
            {
                var dialogService = ServiceHelper.GetDialogService();
                var result = dialogService.ShowOpenFileNameDialog(DefaultExtension);

                if (result.IsConfirmed)
                {
                    fileLogic.InternalLoad(result.File);
                    UpdateFileStatus();
                }
            }

        }

        private void OnSave()
        {
            if (fileLogic != null)
            {
                if (fileLogic.CurrentState == EditorState.New || fileLogic.CurrentState == EditorState.ChangedNew)
                {
                    OnSaveAs();
                }

                // Just save it
                fileLogic.InternalSave(fileLogic.FileName);

                UpdateFileStatus();
            }


        }

        private void OnSaveAs()
        {
            if (fileLogic != null)
            {
                var dialogService = ServiceHelper.GetDialogService();
                var result = dialogService.ShowSaveFileDialog(DefaultExtension);

                if (result.IsConfirmed)
                {
                    fileLogic.InternalSave(result.File);
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

                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON file is formatted", false));
                }
                catch (FeatureException featureException)
                {
                    ServiceHelper.ShowError(featureException, JsonComponent);
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON operation failed", true));
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

                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON content rendered to ScratchPad", false));
                }
                catch (FeatureException featureException)
                {
                    ServiceHelper.ShowError(featureException, JsonComponent);
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON operation failed", true));
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
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON content rendered to tree", false));
                }
                catch (FeatureException featureException)
                {
                    ServiceHelper.ShowError(featureException, JsonComponent);
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON operation failed", true));
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
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML file formatted", false));
                }
                catch (FeatureException featureException)
                {
                    ServiceHelper.ShowError(featureException, XmlComponent);
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML operation failed", true));
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

                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML content rendered to ScratchPad", false));
                }
                catch (FeatureException featureException)
                {
                    ServiceHelper.ShowError(featureException, JsonComponent);
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML operation failed", true));
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
                    Ui!.AddToScratchPad(content);
                }
            }
        }

        private void OnFind()
        {
            var dialogService = ServiceHelper.GetDialogService();
            dialogService.OpenFindDialog(Ui);
        }

        private void OnReplace()
        {
            var dialogService = ServiceHelper.GetDialogService();
            dialogService.OpenReplaceDialog(Ui);
        }

        private void OnCopyToScratchPad()
        {
            var isSelected = Ui!.IsTextSelected();
            var text = Ui.GetText(isSelected);

            Ui.AddToScratchPad(text);
        }

        private void OnText(TextActionEnum textAction)
        {
            if (fileLogic != null)
            {
                fileLogic.InternalText(textAction);
            }
            
        }

        private void OnTextClipboard(ClipboardActionEnum action)
        {
            if (fileLogic != null)
            {
                fileLogic.PerfromClipboardAction(action);
            }
        }

        #endregion

        #region IMainViewModel

        public void Init(IMainViewUi ui)
        {
            Ui = ui;
            fileLogic = new InternalFileLogic(ui);

            fileLogic.InternalNew();
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
            if (fileLogic != null)
            {
                int internalTextLength = fileLogic.InitialText.Length;
                var loadedEvent = offset == 0 && internalTextLength == added;

                var isChangedSelected = fileLogic.CurrentState == EditorState.Changed || fileLogic.CurrentState == EditorState.ChangedNew;
                if (!loadedEvent && !isChangedSelected)
                {
                    if (fileLogic.CurrentState == EditorState.New)
                    {
                        fileLogic.CurrentState = EditorState.ChangedNew;
                    }
                    else
                    {
                        fileLogic.CurrentState = EditorState.Changed;
                    }

                    UpdateFileStatus();
                }
            }
        }

        public bool IsChanged()
        {
            if (fileLogic != null)
            {
                var isChangeRequired = fileLogic.CurrentState == EditorState.Changed || fileLogic.CurrentState == EditorState.ChangedNew;
                return isChangeRequired;
            }

            return false;

        }

        #endregion

        private void UpdateFileStatus()
        {
            var currentState = "Unknown";

            State = currentState;
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
            Cut = new DefaultCommand(() => OnTextClipboard(ClipboardActionEnum.Cut));
            // Copy
            Copy = new DefaultCommand(() => OnTextClipboard(ClipboardActionEnum.Copy));
            // Paste
            Paste = new DefaultCommand(() => OnTextClipboard(ClipboardActionEnum.Paste));
            // Select All
            SelectAll = new DefaultCommand(() => OnTextClipboard(ClipboardActionEnum.SelectAll));

            //Tools
            JsonFormatter = new DefaultCommand(OnJsonFormatter);
            JsonToStringParser = new DefaultCommand(OnJsonToString);
            JsonToTreeParser = new DefaultCommand(OnJsonToTree);
            XmlFormatter = new DefaultCommand(OnXmlFormatter);
            XmlToStringParser = new DefaultCommand(OnXmlToString);
            XmlToTreeParser = new DefaultCommand(OnXmlToTree);
            TextSplit = new DefaultCommand(()=>OnText(TextActionEnum.Split));
            TextGroup = new DefaultCommand(() => OnText(TextActionEnum.Group));
            TextToLower = new DefaultCommand(() => OnText(TextActionEnum.ToLower));
            TextToUpper = new DefaultCommand(() => OnText(TextActionEnum.ToUpper));
            TextTrim = new DefaultCommand(() => OnText(TextActionEnum.Trim));
            TextCountLength = new DefaultCommand(() => OnText(TextActionEnum.LengthCount));

            // Layout
            ToggleLineWrap = new DefaultCommand(OnToggleTextWrap);
            //Refresh = new DefaultCommand(OnRefresh);

            // ScratchPad
            ScratchPadClearAll = new DefaultCommand(OnClearAllScratchPad);
            ScratchPadClearText = new DefaultCommand(OnClearTextScratchPad);
            ScratchPadClearTree = new DefaultCommand(OnClearTreeScratchPad);
            ScratchPadCopyClipboard = new DefaultCommand(OnCopyClipboardToScratchPad);

            // About
            About = new DefaultCommand(OnAbout);
        }

    }
}

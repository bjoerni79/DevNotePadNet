using DevNotePad.Features;
using DevNotePad.Features.Json;
using DevNotePad.Features.Shared;
using DevNotePad.Features.Xml;
using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared;
using DevNotePad.Shared.Event;
using Generic.MVVM;
using Generic.MVVM.Event;
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
    public class MainViewModel : AbstractViewModel, IMainViewModel, IEventListener
    {
        private readonly string ApplicationComponent = "Application";
        private readonly string JsonComponent = "JSON";
        private readonly string XmlComponent = "XML";

        private const string TextDefaultExtension = "All|*.*|Text Files|*.txt|Log Files|*.log|XML|*.xml;*.xsd;*.xslt|JSON|*.json|SQL|*.sql";
        private const string BinaryDefaultExtension = "All|*.*|bin|*.bin";
        private IMainViewUi? Ui { get; set; }

        private ITextComponent? textComponent;

        private ITextComponent? scratchPadComponent;

        private IFileLogic? textLogic;

        private IFileLogic? scratchPadLogic;

        private CommandGroup fileGroup;
        private CommandGroup textOperationGroup;
        private CommandGroup scratchOperationGroup;

        public bool LineWrapMode { get; private set; }

        public bool ScratchPadMode { get; private set; }


        public string State { get; private set; }

        public bool IsStateChanged { get; private set; }

        public MainViewModel()
        {
            State = "Ready";
            IsStateChanged = false;
            InitMenu();

        }

        #region Commands

        // File

        public IRefreshCommand? New { get; private set; }

        public IRefreshCommand? Open { get; private set; }

        public IRefreshCommand? OpenBinary { get; private set; }

        public IRefreshCommand? Save { get; private set; }

        public IRefreshCommand? SaveBinary { get; private set; }

        public IRefreshCommand? SaveAs { get; private set; }

        public IRefreshCommand? SaveAsBinary { get; private set; }

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

        public IRefreshCommand? ToggleScratchPad { get; private set; }




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

        public IRefreshCommand? TextHexCountLength { get; private set; }

        public IRefreshCommand? TextFormatHex { get; private set; }

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

        public IRefreshCommand? ScratchPadHexCountLength { get; private set; }

        public IRefreshCommand? ScratchPadFormatHex { get; private set; }

        public IRefreshCommand? SchemaValidatorTool { get; private set; }

        public IRefreshCommand? Base64Tool { get; private set; }

        public IRefreshCommand? About { get; private set; }

        public IRefreshCommand? Refresh { get; private set; }

        public IRefreshCommand? DecodeTlv { get; private set; }

        public IRefreshCommand? AppletTool { get; private set; }

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
                if (operation == FileOperation.Open || operation == FileOperation.OpenBinary)
                {
                    string extensions = TextDefaultExtension;
                    if (operation == FileOperation.OpenBinary)
                    {
                        extensions = BinaryDefaultExtension;
                    }

                    var result = dialogService.ShowOpenFileNameDialog(extensions);
                    if (result.IsConfirmed)
                    {
                        if (operation == FileOperation.Open)
                        {
                            textLogic.Load(result.File);
                        }
                        else
                        {
                            textLogic.LoadBinary(result.File);
                        }
                    }
                }

                // File Save
                if (operation == FileOperation.Save)
                {
                    if (textLogic.CurrentState == EditorState.New || textLogic.CurrentState == EditorState.ChangedNew)
                    {
                        var result = dialogService.ShowSaveFileDialog(TextDefaultExtension);
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

                // File Save Binary
                if (operation == FileOperation.SaveBinary)
                {
                    if (textLogic.CurrentState == EditorState.New || textLogic.CurrentState == EditorState.ChangedNew)
                    {
                        var result = dialogService.ShowSaveFileDialog(BinaryDefaultExtension);
                        if (result.IsConfirmed)
                        {
                            textLogic.SaveBinary(result.File);
                        }
                    }
                    else
                    {
                        textLogic.SaveBinary(textLogic.FileName);
                    }
                }

                // File Save As Binary
                if (operation == FileOperation.SaveAsBinary)
                {
                    var result = dialogService.ShowSaveFileDialog(BinaryDefaultExtension);
                    if (result.IsConfirmed)
                    {
                        textLogic.SaveBinary(result.File);
                    }
                }

                // File Save As
                if (operation == FileOperation.SaveAs)
                {
                    var result = dialogService.ShowSaveFileDialog(TextDefaultExtension);
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

        private void OnToggleScratchPad()
        {
            var settings = GetSettings();

            ScratchPadMode = !ScratchPadMode;
            settings.ScratchPadEnabled = ScratchPadMode;

            RaisePropertyChange("ScratchPadMode");
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

                        var dialogService = ServiceHelper.GetToolDialogService();
                        dialogService.OpenTreeView();

                        var updateTreeEvent = ServiceHelper.GetEvent(Bootstrap.UpdateTreeEvent);
                        updateTreeEvent.Trigger(new UpdateTree(new List<ItemNode>() { rootNode, }));

                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("JSON content rendered to tree", false));
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
                        var formatterTask = Task.Run(() => component.FormatterAsync(input));

                        formatterTask.Wait();

                        scratchPadComponent.SetText(formatterTask.Result, isTextSelected);
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML file formatted", false));
                    }

                    if (operation == XmlOperation.ToTree)
                    {
                        var parseToTreeTask = Task.Run(() => component.ParseToTreeAsync(input));
                        parseToTreeTask.Wait();

                        // Open the view and trigger the event
                        var dialogFactory = ServiceHelper.GetToolDialogService();
                        dialogFactory.OpenTreeView();

                        var updateTreeEvent = ServiceHelper.GetEvent(Bootstrap.UpdateTreeEvent);
                        updateTreeEvent.Trigger(new UpdateTree(parseToTreeTask.Result));
                    }
                }
                catch (AggregateException aEx)
                {
                    foreach (var ex in aEx.InnerExceptions)
                    {
                        ServiceHelper.ShowError(ex, XmlComponent);
                    }

                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML operation failed", true));
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

                        var dialogService = ServiceHelper.GetToolDialogService();
                        dialogService.OpenTreeView();

                        var updateTreeEvent = ServiceHelper.GetEvent(Bootstrap.UpdateTreeEvent);
                        updateTreeEvent.Trigger(new UpdateTree(new List<ItemNode>() { rootNode, }));

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
                        var formatterTask = Task.Run(()=>component.FormatterAsync(input));
                        formatterTask.Wait();

                        //var formatted = component.Formatter(input);

                        textComponent.SetText(formatterTask.Result, isTextSelected);
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML file formatted", false));
                    }

                    // To Tree Action
                    if (operation == XmlOperation.ToTree)
                    {
                        var treeNodeTask = Task.Run(()=>component.ParseToTreeAsync(input));
                        treeNodeTask.Wait();

                        // Open the view and trigger the event
                        var dialogFactory = ServiceHelper.GetToolDialogService();
                        dialogFactory.OpenTreeView();

                        var updateTreeEvent = ServiceHelper.GetEvent(Bootstrap.UpdateTreeEvent);
                        updateTreeEvent.Trigger(new UpdateTree(treeNodeTask.Result));
                    }

                }
                catch (AggregateException aEx)
                {
                    foreach (var ex in aEx.InnerExceptions)
                    {
                        ServiceHelper.ShowError(ex, XmlComponent);
                    }

                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML operation failed", true));
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

        private void OnAppletTool()
        {
            var dialogService = ServiceHelper.GetToolDialogService();
            dialogService.OpenAppletToolDialog(Ui, textComponent);
        }

        private void OnBase64Tool()
        {
            var dialogService = ServiceHelper.GetToolDialogService();
            dialogService.OpenBase64Dialog(Ui, textComponent);
        }

        private void OnSchemaXmlValidator()
        {
            var dialogService = ServiceHelper.GetToolDialogService();
            dialogService.OpenXmlSchemaValidatorDialog(Ui, textComponent);
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
            var dialogService = ServiceHelper.GetToolDialogService();
            dialogService.OpenFindDialog(Ui,textComponent);
        }

        private void OnReplace()
        {
            var dialogService = ServiceHelper.GetToolDialogService();
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
                logic.PerformTextAction(textAction);
            }
        }

        private void OnDecodeTlv()
        {
            if (textComponent != null)
            {
                bool isSelected = textComponent.IsTextSelected();
                if (isSelected)
                {
                    try
                    {
                        var tlvDecoder = FeatureFactory.CreateTlvDecoder();
                        var textFormatter = FeatureFactory.CreateTextFormat();

                        // Convert the hex bytes to bytes. 
                        // A FormatException is thrown if not possible
                        var selectedText = textComponent.GetText(isSelected);
                        var groupedCoding = textFormatter.GroupString(selectedText);
                        var hexBytes = Convert.FromHexString(groupedCoding);

                        // Try to decode the TLV
                        var tlv = tlvDecoder.Decode(hexBytes);

                        var tag = Convert.ToHexString(tlv.TagBytes);
                        var length = Convert.ToHexString(tlv.LengthBytes);
                        
                        
                        // Format the byte
                        var formattedTlvBuilder = new StringBuilder();
                        formattedTlvBuilder.AppendFormat("{0}  {1}\n",tag,length);

                        if (tlv.ByteValue.Any())
                        {
                            var value = Convert.ToHexString(tlv.ByteValue);
                            formattedTlvBuilder.AppendFormat("  {0}\n", value);
                        }
                        
                        if (tlv.RemainingBytes != null && tlv.RemainingBytes.Any())
                        {
                            var remaining = Convert.ToHexString(tlv.RemainingBytes);
                            formattedTlvBuilder.AppendFormat("{0}\n", remaining);
                        }

                        textComponent.SetText(formattedTlvBuilder.ToString(), true);
                    }
                    catch (FormatException formatException)
                    {
                        ServiceHelper.ShowError(formatException, "TLV Decoder");
                    }
                    catch (FeatureException featureException)
                    {
                        ServiceHelper.ShowError(featureException, "TLV Decoder");
                    }
                    catch (Exception ex)
                    {
                        ServiceHelper.ShowError(ex, "TLV Decoder");
                    }
                }
                else
                {
                    // TODO: Show that no text is selected in the textbox
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("No text selected", false));
                }
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

            InitEvents();
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

                //TODO: Update Ui in regard of ScratchPad
                Ui.SetScratchPad(settings.ScratchPadEnabled);
                scratchOperationGroup.Refresh();
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

        public void NotifyScratchPadContentChanged(int added, int offset, int removed)
        {
            // Simply refresh the UI for now

            scratchOperationGroup.Refresh();
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
            // No UI Thread involved. Can be run in a worker thread.

            var currentUiState = "Unknown";
            bool isChanged = false;
            if (textLogic != null)
            {
                var currentState = textLogic.CurrentState;
                if (currentState == EditorState.Changed || currentState == EditorState.ChangedNew)
                {
                    currentUiState = "Changed";
                    isChanged = true;
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
            IsStateChanged = isChanged;
            RaisePropertyChange("State");
            RaisePropertyChange("IsStateChanged");
            fileGroup.Refresh();
            textOperationGroup.Refresh();
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

        private void InitEvents()
        {
            var facade = FacadeFactory.Create();
            var eventController = facade.Get<EventController>(Bootstrap.EventControllerId);
            if (eventController != null)
            {
                var fileStatsUpdateEvent = eventController.GetEvent(Bootstrap.UpdateFileStateEvent);
                if (fileStatsUpdateEvent != null)
                {
                    fileStatsUpdateEvent.AddListener(this);
                }
            }
        }

        private bool IsText()
        {
            bool isEnabled = false;

            if (textLogic != null)
            {
                return textLogic.IsTextFormatAvailable;
            }
            return isEnabled;
        }

        private bool IsBinary()
        {
            bool isEnabled = false;

            if (textLogic != null)
            {
                var isBinaryActive = !textLogic.IsTextFormatAvailable;
                return isBinaryActive;
            }
            return isEnabled;
        }

        private bool IsReloadFeatureAvailable()
        {
            bool isEnabled = false;

            if (textLogic != null)
            {
                var currentState = textLogic.CurrentState;

                isEnabled = textLogic.IsTextFormatAvailable && currentState != EditorState.New && currentState != EditorState.ChangedNew;
            }

            return isEnabled;
        }

        private bool IsTextOperationEnabled()
        {
            return IsContentAvailable(textComponent);
        }

        private bool IsScratchPadOperationEnabled()
        {
            return ScratchPadMode && IsContentAvailable(scratchPadComponent);
        }

        private bool IsContentAvailable(ITextComponent component)
        {
            bool isEnabled = false;

            if (component != null)
            {
                // enabled if the text is not empty
                var text = component.GetText(false);
                if (!string.IsNullOrEmpty(text))
                {
                    isEnabled = true;
                }
            }

            return isEnabled;
        }

        private void InitFileMenu()
        {
            // File
            New = new DefaultCommand(() => OnFileOperation(FileOperation.New));
            Open = new DefaultCommand(() => OnFileOperation(FileOperation.Open));
            OpenBinary = new DefaultCommand(() => OnFileOperation(FileOperation.OpenBinary));

            Save = new DefaultCommand(() => OnFileOperation(FileOperation.Save), IsText);
            SaveBinary = new DefaultCommand(() => OnFileOperation(FileOperation.SaveBinary), IsBinary);
            SaveAs = new DefaultCommand(() => OnFileOperation(FileOperation.SaveAs), IsText);
            SaveAsBinary = new DefaultCommand(() => OnFileOperation(FileOperation.SaveAsBinary), IsBinary);
            Reload = new DefaultCommand(() => OnFileOperation(FileOperation.Reload), IsReloadFeatureAvailable);
            Close = new DefaultCommand(OnClose);

            // Assign the commands to a group for later refresh actions
            fileGroup = new CommandGroup(new List<IRefreshCommand>()
            {
                Save,
                SaveAs,
                SaveAsBinary,
                SaveBinary,
                Reload
            });
        }

        private void InitEditMenu()
        {
            // Edit
            Find = new DefaultCommand(OnFind,IsTextOperationEnabled);
            Replace = new DefaultCommand(OnReplace,IsTextOperationEnabled);
            CopyToScratchPad = new DefaultCommand(OnCopyToScratchPad, IsTextOperationEnabled);
            Cut = new DefaultCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.Cut),IsTextOperationEnabled);
            Copy = new DefaultCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.Copy), IsTextOperationEnabled);
            Paste = new DefaultCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.Paste));
            SelectAll = new DefaultCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.SelectAll), IsTextOperationEnabled);

            //Tools
            JsonFormatter = new DefaultCommand(() => OnJson(JsonOperation.Format), IsTextOperationEnabled);
            JsonToStringParser = new DefaultCommand(() => OnJson(JsonOperation.ToText), IsTextOperationEnabled);
            JsonToTreeParser = new DefaultCommand(() => OnJson(JsonOperation.ToTree), IsTextOperationEnabled);
            XmlFormatter = new DefaultCommand(() => OnXml(XmlOperation.Format), IsTextOperationEnabled);
            XmlToStringParser = new DefaultCommand(() => OnXml(XmlOperation.ToText), IsTextOperationEnabled);
            XmlToTreeParser = new DefaultCommand(() => OnXml(XmlOperation.ToTree), IsTextOperationEnabled);
            TextSplit = new DefaultCommand(() => OnText(textLogic, TextActionEnum.Split), IsTextOperationEnabled);
            TextGroup = new DefaultCommand(() => OnText(textLogic, TextActionEnum.Group), IsTextOperationEnabled);
            TextToLower = new DefaultCommand(() => OnText(textLogic, TextActionEnum.ToLower), IsTextOperationEnabled);
            TextToUpper = new DefaultCommand(() => OnText(textLogic, TextActionEnum.ToUpper), IsTextOperationEnabled);
            TextTrim = new DefaultCommand(() => OnText(textLogic, TextActionEnum.Trim), IsTextOperationEnabled);
            TextCountLength = new DefaultCommand(() => OnText(textLogic, TextActionEnum.LengthCount), IsTextOperationEnabled);
            TextHexCountLength = new DefaultCommand(() => OnText(textLogic, TextActionEnum.HexLengthCount), IsTextOperationEnabled);
            TextFormatHex = new DefaultCommand(() => OnText(textLogic, TextActionEnum.HexFormat), IsTextOperationEnabled);

            // Assign the commands to a group
            textOperationGroup = new CommandGroup(new List<IRefreshCommand> () {
                Find,
                Replace,
                CopyToScratchPad,
                Cut,
                Copy,
                SelectAll,
                JsonFormatter,
                JsonToStringParser,
                JsonToTreeParser,
                XmlFormatter,
                XmlToStringParser,
                XmlToTreeParser,
                TextSplit,
                TextGroup,
                TextToLower,
                TextToUpper,
                TextTrim,
                TextCountLength,
                TextHexCountLength,
                TextFormatHex
            });
        }

        private void InitScratchPadMenu()
        {
            // ScratchPad
            CopyToText = new DefaultCommand(OnCopyToText,IsScratchPadOperationEnabled);
            ScratchPadClearAll = new DefaultCommand(OnClearAllScratchPad, ()=>ScratchPadMode);
            ScratchPadClearText = new DefaultCommand(OnClearTextScratchPad, ()=>ScratchPadMode);
            ScratchPadCopyClipboard = new DefaultCommand(OnCopyClipboardToScratchPad, IsScratchPadOperationEnabled);
            ScratchPadCopy = new DefaultCommand(() => OnTextClipboard(scratchPadLogic, ClipboardActionEnum.Copy), IsScratchPadOperationEnabled);
            ScratchPadCut = new DefaultCommand(() => OnTextClipboard(scratchPadLogic, ClipboardActionEnum.Cut), IsScratchPadOperationEnabled);
            ScratchPadPaste = new DefaultCommand(() => OnTextClipboard(scratchPadLogic, ClipboardActionEnum.Paste),()=>ScratchPadMode);
            ScratchPadXmlFormat = new DefaultCommand(() => OnScratchPadXml(XmlOperation.Format), IsScratchPadOperationEnabled);
            ScratchPadXmlToTree = new DefaultCommand(() => OnScratchPadXml(XmlOperation.ToTree), IsScratchPadOperationEnabled);
            ScratchPadJsonFormat = new DefaultCommand(() => OnScratchPadJson(JsonOperation.Format), IsScratchPadOperationEnabled);
            ScratchPadJsonToTree = new DefaultCommand(() => OnScratchPadJson(JsonOperation.ToTree), IsScratchPadOperationEnabled);

            //TODO: Text Actions
            ScratchPadSplit = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.Split), IsScratchPadOperationEnabled);
            ScratchPadGroup = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.Group), IsScratchPadOperationEnabled);
            ScratchpadToLower = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.ToLower), IsScratchPadOperationEnabled);
            ScratchPadToUpper = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.ToUpper), IsScratchPadOperationEnabled);
            ScratchPadTrim = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.Trim), IsScratchPadOperationEnabled);
            ScratchPadCountLength = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.LengthCount), IsScratchPadOperationEnabled);
            ScratchPadHexCountLength = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.HexLengthCount), IsScratchPadOperationEnabled);
            ScratchPadFormatHex = new DefaultCommand(() => OnText(scratchPadLogic, TextActionEnum.HexFormat), IsScratchPadOperationEnabled);

            // Assign to the scratchPad operation group
            scratchOperationGroup = new CommandGroup(new List<IRefreshCommand>() {
                CopyToText,
                ScratchPadCopyClipboard,
                ScratchPadCopy,
                ScratchPadCut,
                ScratchPadXmlFormat,
                ScratchPadXmlToTree,
                ScratchPadJsonFormat,
                ScratchPadJsonToTree,
                ScratchPadSplit,
                ScratchPadGroup,
                ScratchpadToLower,
                ScratchPadToUpper,
                ScratchPadTrim,
                ScratchPadCountLength,
                ScratchPadHexCountLength,
                ScratchPadFormatHex,
                ScratchPadClearAll,
                ScratchPadClearText,
                ScratchPadClearTree,
                ScratchPadPaste
            });
        }

        private void InitMenu()
        {
            InitFileMenu();
            InitEditMenu();
            InitScratchPadMenu();

            // Layout
            ToggleLineWrap = new DefaultCommand(OnToggleTextWrap);
            ToggleScratchPad = new DefaultCommand(OnToggleScratchPad);

            //...
            SchemaValidatorTool = new DefaultCommand(OnSchemaXmlValidator);
            Base64Tool = new DefaultCommand(OnBase64Tool);
            DecodeTlv = new DefaultCommand(OnDecodeTlv);
            AppletTool = new DefaultCommand(OnAppletTool);

            // About
            About = new DefaultCommand(OnAbout);
        }

        #region Event Mechanism

        public void OnTrigger(string eventId)
        {
            if (eventId == Bootstrap.UpdateFileStateEvent)
            {
                UpdateFileStatus();
            }
        }

        public void OnTrigger(string eventId, object parameter)
        {
            // None..
        }

        #endregion

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
            OpenBinary,
            Save,
            SaveAs,
            SaveBinary,
            SaveAsBinary,
            Reload
        }
    }
}

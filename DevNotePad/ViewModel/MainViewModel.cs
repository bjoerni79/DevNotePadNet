using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DevNotePad.Features;
using DevNotePad.Features.Json;
using DevNotePad.Features.Shared;
using DevNotePad.Features.Xml;
using DevNotePad.MVVM;
using DevNotePad.Shared;
using DevNotePad.Shared.Event;
using DevNotePad.Shared.Message;
using Generic.MVVM.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class MainViewModel : ObservableRecipient, IMainViewModel
    {
        private readonly string ApplicationComponent = "Application";
        private readonly string JsonComponent = "JSON";
        private readonly string XmlComponent = "XML";

        private const string TextDefaultExtension = "All|*.*|Text Files|*.txt|Log Files|*.log|XML|*.xml;*.xsd;*.xslt|JSON|*.json|SQL|*.sql";
        private const string BinaryDefaultExtension = "All|*.*|bin|*.bin";
        private IMainViewUi? Ui { get; set; }

        private ITextComponent? textComponent;

        private IFileLogic? textLogic;

        private RelayCommandGroup fileGroup;
        private RelayCommandGroup textOperationGroup;
        private RelayCommandGroup scratchOperationGroup;
        private RelayCommandGroup toolGroup;

        public string State { get; private set; }

        public bool IsStateChanged { get; private set; }

        public MainViewModel()
        {
            State = "Ready";
            IsStateChanged = false;
            InitMenu();

        }

        protected override void OnActivated()
        {
            base.OnActivated();

            // https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/observablerecipient
            //
            // Register the two events. The interface method can only deal with one.
            Messenger.Register<MainViewModel, UpdateAsyncProcessStateMessage>(this, (r, m) => InternalUpdateAsync(m.Value));

            Messenger.Register<MainViewModel, UpdateStatusBarParameterMessage>(this, (r, m) => InternalUpdateStatus(m.Value));

            // TODO: Update File Status..
            Messenger.Register<MainViewModel, UpdateFileStatusMessage>(this, (r, m) => UpdateFileStatus(m.Value));
        }

        #region Commands

        // File

        public RelayCommand? New { get; private set; }

        public RelayCommand? Open { get; private set; }

        public RelayCommand? OpenBinary { get; private set; }

        public RelayCommand? Save { get; private set; }

        public RelayCommand? SaveBinary { get; private set; }

        public RelayCommand? SaveAs { get; private set; }

        public RelayCommand? SaveAsBinary { get; private set; }

        public RelayCommand? Reload { get; private set; }

        public RelayCommand? Close { get; private set; }

        // Edit

        public RelayCommand? Find { get; private set; }

        public RelayCommand? Replace { get; private set; }


        public RelayCommand? CopyToText { get; private set; }

        public RelayCommand? Cut { get; private set; }

        public RelayCommand? Copy { get; private set; }

        public RelayCommand? Paste { get; private set; }


        public RelayCommand? SelectAll { get; private set; }

        // Layout

        public RelayCommand? Settings { get; private set; }

        public RelayCommand? JsonFormatter { get; private set; }

        public RelayCommand? JsonToStringParser { get; private set; }

        public RelayCommand? JsonToTreeParser { get; private set; }

        public RelayCommand? XmlFormatter { get; private set; }

        public RelayCommand? XmlToStringParser { get; private set; }

        public RelayCommand? XmlToTreeParser { get; private set; }

        public RelayCommand? TextSplit { get; private set; }

        public RelayCommand? TextGroup { get; private set; }

        public RelayCommand? TextToLower { get; private set; }

        public RelayCommand? TextToUpper { get; private set; }

        public RelayCommand? TextTrim { get; private set; }

        public RelayCommand? TextCountLength { get; private set; }

        public RelayCommand? TextHexCountLength { get; private set; }

        public RelayCommand? TextFormatHex { get; private set; }



        public RelayCommand? SchemaValidatorTool { get; private set; }

        public RelayCommand? XPathQueryTool { get; private set; }

        public RelayCommand? XSltTransformationTool { get; private set; }

        public RelayCommand? RegularExpressionTool { get; private set; }

        public RelayCommand? Base64Tool { get; private set; }

        public RelayCommand? About { get; private set; }

        public RelayCommand? Refresh { get; private set; }

        public RelayCommand? DecodeTlv { get; private set; }

        public RelayCommand? CreateGuid { get; private set; }

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

                        WeakReferenceMessenger.Default.Send(new UpdateTreeMessage(new List<ItemNode>() { rootNode }));

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
                        var formatterTask = Task.Run(() => component.FormatterAsync(input));
                        formatterTask.Wait();

                        //var formatted = component.Formatter(input);

                        textComponent.SetText(formatterTask.Result, isTextSelected);
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("XML file formatted", false));
                    }

                    // To Tree Action
                    if (operation == XmlOperation.ToTree)
                    {
                        var treeNodeTask = Task.Run(() => component.ParseToTreeAsync(input));
                        treeNodeTask.Wait();

                        // Open the view and trigger the event
                        var dialogFactory = ServiceHelper.GetToolDialogService();
                        dialogFactory.OpenTreeView();

                        WeakReferenceMessenger.Default.Send(new UpdateTreeMessage(treeNodeTask.Result));
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

        private void OnRegularExpressionTool()
        {
            var toolService = ServiceHelper.GetToolDialogService();
            toolService.OpenRegularExpressionDialog(Ui, textComponent);
        }

        private void OnBase64Tool()
        {
            var dialogService = ServiceHelper.GetToolDialogService();
            dialogService.OpenBase64Dialog(Ui, textComponent);
        }

        private void OnXmlTool(XmlToolFeature feature)
        {
            var toolDialogService = ServiceHelper.GetToolDialogService();

            switch (feature)
            {
                case XmlToolFeature.SchemaValidation:
                    toolDialogService.OpenXmlSchemaValidatorDialog(Ui, textComponent);
                    break;
                case XmlToolFeature.XPathQuery:
                    toolDialogService.OpenXPathQueryDialog(Ui, textComponent);
                    break;
                case XmlToolFeature.XSltTransformation:
                    toolDialogService.OpenXSltTransfomerDialog(Ui, textComponent);
                    break;
                default:
                    break;
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


        private void OnFind()
        {
            var dialogService = ServiceHelper.GetToolDialogService();
            dialogService.OpenFindDialog(Ui, textComponent);
        }

        private void OnReplace()
        {
            var dialogService = ServiceHelper.GetToolDialogService();
            dialogService.OpenReplaceDialog(Ui, textComponent);
        }

        private void OnText(IFileLogic logic, TextActionEnum textAction)
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
                        formattedTlvBuilder.AppendFormat("{0}  {1}\n", tag, length);

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

        private void OnTextClipboard(IFileLogic logic, ClipboardActionEnum action)
        {
            if (logic != null)
            {
                logic.PerfromClipboardAction(action);
            }
        }

        private void OnSettings()
        {
            var dialogService = ServiceHelper.GetDialogService();
            dialogService.ShowSettings();
            ApplySettings();
        }

        private void OnCreateGuid()
        {
            var toolDialogService = ServiceHelper.GetToolDialogService();
            toolDialogService.OpenGuidDialog(Ui, textComponent);
        }

        #endregion

        #region IMainViewModel

        public void Init(IMainViewUi ui, ITextComponent text)
        {
            Ui = ui;
            textComponent = text;

            textLogic = new InternalFileLogic(Ui, textComponent);
            textLogic.New();

            UpdateFileStatus();
        }

        public void ApplySettings()
        {
            var settings = ServiceHelper.GetSettings();

            if (Ui != null)
            {
                //Ui.SetScrollbars(ScrollbarMode);
                Ui.SetWordWrap(settings.LineWrap);

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
            var isChanged = false;
            var settings = ServiceHelper.GetSettings();
            var ignoreChanges = settings.IgnoreChanged;

            if (!ignoreChanges && textLogic != null)
            {
                isChanged = textLogic.CurrentState == EditorState.Changed || textLogic.CurrentState == EditorState.ChangedNew;
            }

            return isChanged;
        }

        public void OpenExternalFile(string filePath)
        {
            // Check if the file exists
            if (!String.IsNullOrEmpty(filePath))
            {
                var fileExists = File.Exists(filePath);
                if (fileExists)
                {
                    // Open it
                    textLogic.Load(filePath);
                }
            }
        }

        #endregion

        private void UpdateFileStatus(EditorState state)
        {
            var currentUiState = "Unknown";
            bool isChanged = false;

            if (state == EditorState.Changed || state == EditorState.ChangedNew)
            {
                currentUiState = "Changed";
                isChanged = true;
            }

            if (state == EditorState.New)
            {
                currentUiState = "New";
            }

            if (state == EditorState.Saved)
            {
                currentUiState = "Saved";
            }

            if (state == EditorState.Loaded)
            {
                currentUiState = "Loaded";
            }

            // Notify the UI
            State = currentUiState;
            IsStateChanged = isChanged;
            OnPropertyChanged("State");
            OnPropertyChanged("IsStateChanged");

            fileGroup.Refresh();
            textOperationGroup.Refresh();
            toolGroup.Refresh();
        }

        /// <summary>
        /// Updates the UI with the current state in the file logic
        /// </summary>
        private void UpdateFileStatus()
        {
            // No UI Thread involved. Can be run in a worker thread.

            if (textLogic != null)
            {
                var currentState = textLogic.CurrentState;
                UpdateFileStatus(currentState);
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
            return IsText() && IsContentAvailable(textComponent);
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
            New = new RelayCommand(() => OnFileOperation(FileOperation.New));
            Open = new RelayCommand(() => OnFileOperation(FileOperation.Open));
            OpenBinary = new RelayCommand(() => OnFileOperation(FileOperation.OpenBinary));

            Save = new RelayCommand(() => OnFileOperation(FileOperation.Save), IsText);
            SaveBinary = new RelayCommand(() => OnFileOperation(FileOperation.SaveBinary), IsBinary);
            SaveAs = new RelayCommand(() => OnFileOperation(FileOperation.SaveAs), IsText);
            SaveAsBinary = new RelayCommand(() => OnFileOperation(FileOperation.SaveAsBinary), IsBinary);
            Reload = new RelayCommand(() => OnFileOperation(FileOperation.Reload), IsReloadFeatureAvailable);
            Close = new RelayCommand(OnClose);

            // Assign the commands to a group for later refresh actions
            fileGroup = new RelayCommandGroup(new List<RelayCommand>()
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
            Find = new RelayCommand(OnFind, IsTextOperationEnabled);
            Replace = new RelayCommand(OnReplace, IsTextOperationEnabled);
            Cut = new RelayCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.Cut), IsTextOperationEnabled);
            Copy = new RelayCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.Copy), IsTextOperationEnabled);
            Paste = new RelayCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.Paste));
            SelectAll = new RelayCommand(() => OnTextClipboard(textLogic, ClipboardActionEnum.SelectAll), IsTextOperationEnabled);

            //Tools
            JsonFormatter = new RelayCommand(() => OnJson(JsonOperation.Format), IsTextOperationEnabled);
            JsonToStringParser = new RelayCommand(() => OnJson(JsonOperation.ToText), IsTextOperationEnabled);
            JsonToTreeParser = new RelayCommand(() => OnJson(JsonOperation.ToTree), IsTextOperationEnabled);
            XmlFormatter = new RelayCommand(() => OnXml(XmlOperation.Format), IsTextOperationEnabled);
            XmlToStringParser = new RelayCommand(() => OnXml(XmlOperation.ToText), IsTextOperationEnabled);
            XmlToTreeParser = new RelayCommand(() => OnXml(XmlOperation.ToTree), IsTextOperationEnabled);
            TextSplit = new RelayCommand(() => OnText(textLogic, TextActionEnum.Split), IsTextOperationEnabled);
            TextGroup = new RelayCommand(() => OnText(textLogic, TextActionEnum.Group), IsTextOperationEnabled);
            TextToLower = new RelayCommand(() => OnText(textLogic, TextActionEnum.ToLower), IsTextOperationEnabled);
            TextToUpper = new RelayCommand(() => OnText(textLogic, TextActionEnum.ToUpper), IsTextOperationEnabled);
            TextTrim = new RelayCommand(() => OnText(textLogic, TextActionEnum.Trim), IsTextOperationEnabled);
            TextCountLength = new RelayCommand(() => OnText(textLogic, TextActionEnum.LengthCount), IsTextOperationEnabled);
            TextHexCountLength = new RelayCommand(() => OnText(textLogic, TextActionEnum.HexLengthCount), IsTextOperationEnabled);
            TextFormatHex = new RelayCommand(() => OnText(textLogic, TextActionEnum.HexFormat), IsTextOperationEnabled);

            // Assign the commands to a group
            textOperationGroup = new RelayCommandGroup(new List<RelayCommand>() {
                Find,
                Replace,
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

        private void InitMenu()
        {
            InitFileMenu();
            InitEditMenu();

            // Layout
            Settings = new RelayCommand(OnSettings);

            //...
            SchemaValidatorTool = new RelayCommand(() => OnXmlTool(XmlToolFeature.SchemaValidation), IsText);
            XPathQueryTool = new RelayCommand(() => OnXmlTool(XmlToolFeature.XPathQuery), IsText);
            XSltTransformationTool = new RelayCommand(() => OnXmlTool(XmlToolFeature.XSltTransformation), IsText);
            CreateGuid = new RelayCommand(OnCreateGuid);

            RegularExpressionTool = new RelayCommand(() => OnRegularExpressionTool(), IsText);
            Base64Tool = new RelayCommand(OnBase64Tool);
            DecodeTlv = new RelayCommand(OnDecodeTlv, IsText);

            // About
            About = new RelayCommand(OnAbout);

            toolGroup = new RelayCommandGroup(new RelayCommand[] { SchemaValidatorTool, XPathQueryTool, XSltTransformationTool, DecodeTlv });
        }

        private void InternalUpdateAsync(bool isInAsync)
        {
            Ui.UpdateAsyncState(isInAsync);
        }

        private void InternalUpdateStatus(UpdateStatusBarParameter parameter)
        {
            Ui.UpdateToolBar(parameter);
        }
    }
}

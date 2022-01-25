using DevNotePad.Features;
using DevNotePad.MVVM;
using DevNotePad.Shared;
using DevNotePad.Shared.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    internal class InternalFileLogic : IFileLogic
    {
        private ITextComponent textComponent;
        private IMainViewUi mainUi;

        public bool IsTextFormatAvailable { get; private set; }

        internal InternalFileLogic(IMainViewUi ui,ITextComponent textComponent)
        {
            this.textComponent = textComponent;
            this.mainUi = ui;
            InitialText = String.Empty;
            LatestTimeStamp = DateTime.Now;
            FileName = "Unknown";
            IsTextFormatAvailable = true;
        }

        public string InitialText { get; set; }
        public DateTime LatestTimeStamp { get; set; }
        public string FileName { get; set; }


        public EditorState CurrentState { get; set; }

        public void PerformTextAction(TextActionEnum textAction)
        {
            var isSelected = textComponent.IsTextSelected();
            if (!isSelected)
            {
                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("No Text selected. Please select a text first", true));
            }
            else
            {
                try
                {
                    var formatter = FeatureFactory.CreateTextFormat();

                    bool isWarnung = false;
                    bool doUpdate = true;
                    string notifier;

                    var text = textComponent.GetText(true);
                    string formattedText;
                    switch (textAction)
                    {
                        case TextActionEnum.ToLower:
                            formattedText = formatter.ToLower(text);
                            notifier = "Converted to lower chars";
                            break;
                        case TextActionEnum.ToUpper:
                            formattedText = formatter.ToUpper(text);
                            notifier = "Converted to upper chars";
                            break;
                        case TextActionEnum.Group:
                            formattedText = formatter.GroupString(text);
                            notifier = "grouped all chars";
                            break;
                        case TextActionEnum.Split:
                            formattedText = formatter.SplitString(text);
                            notifier = "splitted the chars";
                            break;
                        case TextActionEnum.Trim:
                            formattedText = formatter.Trim(text);
                            notifier = "trimmed the start and the end";
                            break;
                        case TextActionEnum.LengthCount:
                            notifier = formatter.CountLength(text);
                            doUpdate = false;
                            formattedText = text;
                            break;
                        case TextActionEnum.HexLengthCount:
                            notifier = formatter.CountLength(text,true);
                            doUpdate = false;
                            formattedText = text;
                            break;
                        default:
                            formattedText = text;
                            isWarnung = true;
                            notifier = "Unknown Text action detected";
                            break;
                    }

                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter(notifier, isWarnung));

                    if (doUpdate)
                    {
                        textComponent.SetText(formattedText, true);
                    }

                }
                catch (Exception ex)
                {
                    ServiceHelper.ShowError(ex, "Text");
                }
            }
        }

        public void PerfromClipboardAction(ClipboardActionEnum action)
        {
            textComponent.PerformClipboardAction(action);
        }

        /// <summary>
        /// Handles the internal save of the current Text and is called by Save and Save As
        /// </summary>
        /// <param name="filename">the filename</param>
        public bool Save(string targetfilename)
        {
            if (!IsTextFormatAvailable)
            {
                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Please save the content as binary", true));
                return false;
            }

            bool isSuccessful = false;
            try
            {
                var ioService = ServiceHelper.GetIoService();

                // Check for update! What happens if the file is newer than the latest load?
                var fileExists = ioService.ExistFile(targetfilename);
                bool updateDetected = false;
                if (fileExists)
                {
                    var latest = ioService.GetModificationTimeStamp(targetfilename);
                    if (latest > LatestTimeStamp)
                    {
                        updateDetected = true;
                    }
                }

                bool doSave = true;
                if (updateDetected)
                {
                    var dialogService = ServiceHelper.GetDialogService();
                    var result = dialogService.ShowConfirmationDialog("The file in the file system is newer. Do you want to continue?", "Conflict dectected", "Save content");

                    doSave = result;
                }

                if (doSave)
                {
                    var textToSave = textComponent.GetText(false);
                    var saveTask = ioService.WriteTextFileAsync(targetfilename, textToSave);

                    Task.Run(() => ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(true))).
                        ContinueWith((t)=>saveTask).
                        ContinueWith((t)=>
                    {
                        if (!t.IsFaulted)
                        {
                            InitialText = textToSave;
                            CurrentState = EditorState.Saved;
                            LatestTimeStamp = DateTime.Now;
                            FileName = targetfilename;
                            mainUi.SetFilename(FileName);
                            IsTextFormatAvailable = true;
                            isSuccessful = true;
                            ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Content is saved", false));
                        }

                        ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
                    });

                    //// Old Code
                    //InitialText = textComponent.GetText(false);
                    //ioService.WriteTextFile(targetfilename, InitialText);
                    //CurrentState = EditorState.Saved;
                    //LatestTimeStamp = DateTime.Now;

                    //FileName = targetfilename;
                    //mainUi.SetFilename(FileName);

                    //IsTextFormatAvailable = true;
                    //isSuccessful = true;
                    //ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Content is saved", false));
                }
                else
                {
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Content is NOT saved", true));
                }
            }
            catch (Exception ex)
            {
                ServiceHelper.ShowError(ex, "Save File");
            }

            return isSuccessful;
        }

        public bool SaveBinary(string targetFilename)
        {
            var isSuccessful = false;
            var ioService = ServiceHelper.GetIoService();

            try
            {
                FileName = targetFilename;
                InitialText = textComponent.GetText(false);

                Task.Run<Memory<byte>>(() =>
                {
                    ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(true));

                    // Group the chars and numbers first before writing. Convert.FromHexString() throws a FormatException if any parsing errors are occuring.
                    var textFormatComponent = FeatureFactory.CreateTextFormat();
                    var grouped = textFormatComponent.GroupString(InitialText);
                    var byteCoding = Convert.FromHexString(grouped);

                    //TODO: Deal with FormatExceptions!

                    return byteCoding;
                }).ContinueWith((t) => ioService.WriteBinaryAsync(FileName, t.Result)).
                ContinueWith(t => {
                    IsTextFormatAvailable = false;
                    CurrentState = EditorState.Saved;
                    isSuccessful = true;

                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Binary content is saved", false));
                    ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
                });

                //ioService.WriteBinary(FileName, byteCoding);

                //IsTextFormatAvailable = false;
                //CurrentState = EditorState.Saved;
                //isSuccessful = true;
                
            }
            catch (FormatException)
            {
                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Content is not a valid hex format. Save Binary operation failed", true));
            }
            catch (Exception ex)
            {
                ServiceHelper.ShowError(ex, "Save File");
            }

            return isSuccessful;
        }

        /// <summary>
        /// Handles the internal load of files and is called by ICommand delegates
        /// </summary>
        /// <param name="filename">the filename</param>
        public bool Load(string sourceFilename)
        {
            bool isSuccessful = false;

            try
            {
                var ioService = ServiceHelper.GetIoService();
                FileName = sourceFilename;
                CurrentState = EditorState.Loaded;

                //Store the timestamp of the file right now
                LatestTimeStamp = ioService.GetModificationTimeStamp(FileName);

                var readTask = ioService.ReadTextFileAsync(FileName);

                Task.Run(() => ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(true))).
                    ContinueWith((t) => readTask).
                    ContinueWith((t) => 
                    {
                        var readerTask = t.Result;
                        InitialText = readTask.Result;
                        textComponent.SetText(InitialText);
                        mainUi.SetFilename(FileName);
                        IsTextFormatAvailable = true;
                        isSuccessful = true;
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("File is loaded", false));
                        ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
                    });

                //InitialText = ioService.ReadTextFile(FileName);
                //textComponent.SetText(InitialText);
                //mainUi.SetFilename(FileName);

                //IsTextFormatAvailable = true;
                //isSuccessful=true;
                //ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("File is loaded", false));
            }
            catch (Exception ex)
            {
                ServiceHelper.ShowError(ex, "Load File");
            }

            return isSuccessful;
        }

        public bool LoadBinary(string sourceFilename)
        {
            bool isSuccessful = false;

            try
            {
                var ioService = ServiceHelper.GetIoService();
                FileName = sourceFilename;
                CurrentState= EditorState.Loaded;

                LatestTimeStamp = ioService.GetModificationTimeStamp(FileName);

                Task.Run(() => ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(true))).
                    ContinueWith(t => ioService.ReadBinaryAsync(FileName)).
                    ContinueWith(t =>
                    {
                        var loaderTask = t.Result;
                        //TODO: Verify..

                        var byteContent = loaderTask.Result;
                        var hexContent = ToHexStringRowForAsnyc(byteContent);

                        InitialText = hexContent;
                        textComponent.SetText(InitialText);
                        mainUi.SetFilename(FileName);

                        IsTextFormatAvailable = false;
                        isSuccessful = true;

                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("File is loaded as Binary", false));
                        ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
                    });


                // Old
                //var byteContent = ioService.ReadBinary(FileName);
                //var hexContent = ToHexStringRow(byteContent);

                //InitialText=hexContent;
                //textComponent.SetText(InitialText);
                //mainUi.SetFilename(FileName);

                //IsTextFormatAvailable = false;
                //isSuccessful = true;
                //ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("File is loaded as Binary", false));
            }
            catch(Exception ex)
            {
                ServiceHelper.ShowError(ex, "Load File");
            }

            return isSuccessful;
        }

        //TODO: This works for now. Needs review! If this is stable, the span<byte> version can removed
        private string ToHexStringRowForAsnyc(Memory<byte> byteContent)
        {
            int offset = 0;
            int hexBytePerGroup = 16;
            int currentGroupsPerRow = 0;
            int groupsPerRow = 3;
            int length = byteContent.Length;

            var stringBuilder = new StringBuilder();

            // Build a row with 16 bytes each
            while ((offset + hexBytePerGroup) < length)
            {
                var bytesInRow = byteContent.Slice(offset, hexBytePerGroup);
                var rowHexCoding = Convert.ToHexString(bytesInRow.ToArray());

                if (currentGroupsPerRow + 1 >= groupsPerRow)
                {
                    stringBuilder.AppendFormat("{0}\n", rowHexCoding);
                    currentGroupsPerRow = 0;
                }
                else
                {
                    currentGroupsPerRow++;
                    stringBuilder.AppendFormat("{0}  ", rowHexCoding);
                }

                offset += hexBytePerGroup;
            }

            // Add the last bytes at the end
            int lastRowOffset = length - offset;
            if (lastRowOffset > 0)
            {
                var lastRow = byteContent.Slice(offset);
                var lastRowHexCoding = Convert.ToHexString(lastRow.ToArray());

                if (currentGroupsPerRow + 1 >= groupsPerRow)
                {
                    stringBuilder.AppendFormat("{0}\n", lastRowHexCoding);
                }
                else
                {
                    stringBuilder.AppendFormat("{0}", lastRowHexCoding);
                }

            }

            return stringBuilder.ToString();
        }

        private string ToHexStringRow(Span<byte> byteContent)
        {
            int offset = 0;
            int hexBytePerGroup = 16;
            int currentGroupsPerRow = 0;
            int groupsPerRow = 3;
            int length = byteContent.Length;
            
            var stringBuilder = new StringBuilder();

            // Build a row with 16 bytes each
            while ((offset+hexBytePerGroup) < length )
            {
                var bytesInRow = byteContent.Slice(offset, hexBytePerGroup);
                var rowHexCoding = Convert.ToHexString(bytesInRow);

                if (currentGroupsPerRow + 1 >= groupsPerRow)
                {
                    stringBuilder.AppendFormat("{0}\n", rowHexCoding);
                    currentGroupsPerRow = 0;
                }
                else
                {
                    currentGroupsPerRow++;
                    stringBuilder.AppendFormat("{0}  ", rowHexCoding);
                }

                offset += hexBytePerGroup;
            }

            // Add the last bytes at the end
            int lastRowOffset = length - offset;
            if (lastRowOffset > 0)
            {
                var lastRow = byteContent.Slice(offset);
                var lastRowHexCoding = Convert.ToHexString(lastRow);

                if (currentGroupsPerRow + 1 >= groupsPerRow )
                {
                    stringBuilder.AppendFormat("{0}\n", lastRowHexCoding);
                }
                else
                {
                    stringBuilder.AppendFormat("{0}", lastRowHexCoding);
                }
                
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Handles the creation of new files
        /// </summary>
        public bool New()
        {
            bool proceed = true;

            if (CurrentState == EditorState.ChangedNew || CurrentState == EditorState.Changed)
            {
                var dialogService = ServiceHelper.GetDialogService();
                proceed = dialogService.ShowConfirmationDialog("The text is not saved yet. Do you want to continue?", "New", "Create New");
            }

            if (proceed)
            {
                FileName = "New";
                CurrentState = EditorState.New;
                InitialText = String.Empty;
                LatestTimeStamp = DateTime.Now;
                IsTextFormatAvailable = true;

                textComponent.SetText(InitialText);
                mainUi.SetFilename(FileName);

                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("New file created", false));
            }

            return proceed;
        }

        /// <summary>
        /// Handles the reload of a file
        /// </summary>
        public void Reload()
        {
            if (!IsTextFormatAvailable)
            {
                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Reload feature is not available for binary mode", true));
                return;
            }

            if (CurrentState == EditorState.New)
            {
                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Please save the file first", false));
            }
            else
            {
                try
                {
                    // Any file changes?  What is the creation date? etc
                    var io = ServiceHelper.GetIoService();
                    var currentModifiedTimestamp = io.GetModificationTimeStamp(FileName);

                    if (currentModifiedTimestamp > LatestTimeStamp)
                    {
                        bool doReload = true;
                        if (CurrentState == EditorState.Changed || CurrentState == EditorState.ChangedNew)
                        {
                            var dialogService = ServiceHelper.GetDialogService();
                            doReload = dialogService.ShowConfirmationDialog("The text is not saved yet. Do you want to reload?", "Reload", "Reload Content");
                        }


                        if (doReload)
                        {
                            Load(FileName);
                        }
                        else
                        {
                            ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Reload cancelled", true));
                        }
                    }
                    else
                    {
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Reload not required", false));
                    }

                }
                catch (Exception ex)
                {
                    ServiceHelper.ShowError(ex, "Reload");
                }
            }
        }


    }
}

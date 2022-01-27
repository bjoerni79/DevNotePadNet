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
        public void Save(string targetfilename)
        {
            if (!IsTextFormatAvailable)
            {
                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Please save the content as binary", true));
            }

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
                    try
                    {
                        Task.Run(async () => {
                            ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(true));

                            var textToSave = textComponent.GetText(false);
                            await ioService.WriteTextFileAsync(targetfilename, textToSave);

                            InitialText = textToSave;
                            CurrentState = EditorState.Saved;
                            LatestTimeStamp = DateTime.Now;
                            FileName = targetfilename;
                            mainUi.SetFilename(FileName);
                            IsTextFormatAvailable = true;

                            ServiceHelper.TriggerFileUpdate();
                            ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Content is saved", false));

                            ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
                        });
                    }
                    catch (AggregateException aEx)
                    {
                        var exceptions = aEx.InnerExceptions;
                        foreach (var inner in exceptions)
                        {
                            ServiceHelper.ShowError(inner, "Save File");
                        }

                        ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
                    }
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

        }

        public void SaveBinary(string targetFilename)
        {
            var ioService = ServiceHelper.GetIoService();

            try
            {
                FileName = targetFilename;
                InitialText = textComponent.GetText(false);

                // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-6.0

                Task.Run(async () =>
                {
                    ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(true));

                    // Group the chars and numbers first before writing. Convert.FromHexString() throws a FormatException if any parsing errors are occuring.
                    var textFormatComponent = FeatureFactory.CreateTextFormat();
                    var grouped = textFormatComponent.GroupString(InitialText);

                    // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/chaining-tasks-by-using-continuation-tasks

                    byte[]? byteCoding = null;
                    try
                    {
                        // Throws a Format Exception if the content is not a valid hex string
                        byteCoding = Convert.FromHexString(grouped);
                    }
                    catch (FormatException)
                    {
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Content is not a valid hex format. Save Binary operation failed", true));
                        // Ignore.  byteCoding array is null.
                    }

                    if (byteCoding != null)
                    {
                        await ioService.WriteBinaryAsync(FileName, byteCoding);

                        // Write operation done. Update the states now

                        IsTextFormatAvailable = false;
                        CurrentState = EditorState.Saved;

                        ServiceHelper.TriggerFileUpdate();
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Binary content is saved", false));
                    }

                    ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
                });
            }
            catch (AggregateException aEx)
            {
                var exceptions = aEx.InnerExceptions;
                foreach (var inner in exceptions)
                {
                    if (inner is FormatException)
                    {
                        ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("Content is not a valid hex format. Save Binary operation failed", true));
                    }
                    else
                    {
                        ServiceHelper.ShowError(inner, "Save File");
                    }
                }

                ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));

                // https://docs.microsoft.com/en-us/dotnet/api/system.aggregateexception?f1url=%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(System.AggregateException);k(DevLang-csharp)%26rd%3Dtrue&view=net-6.0
            }
            catch (Exception ex)
            {
                ServiceHelper.ShowError(ex, "Save File");
                ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
            }
        }

        /// <summary>
        /// Handles the internal load of files and is called by ICommand delegates
        /// </summary>
        /// <param name="filename">the filename</param>
        public void Load(string sourceFilename)
        {
            try
            {
                var ioService = ServiceHelper.GetIoService();
                FileName = sourceFilename;
                CurrentState = EditorState.Loaded;

                //Store the timestamp of the file right now
                LatestTimeStamp = ioService.GetModificationTimeStamp(FileName);

                Task.Run(async () =>
                {
                    ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(true));

                    var content = await ioService.ReadTextFileAsync(FileName);
                    InitialText = content;
                    textComponent.SetText(InitialText);
                    mainUi.SetFilename(FileName);
                    IsTextFormatAvailable = true;

                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("File is loaded", false));
                    ServiceHelper.TriggerFileUpdate();
                    ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
                });

            }
            catch (AggregateException aEx)
            {
                var exceptions = aEx.InnerExceptions;
                foreach (var inner in exceptions)
                {
                    ServiceHelper.ShowError(inner, "Load File");
                }

                ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
            }
            catch (Exception ex)
            {
                ServiceHelper.ShowError(ex, "Load File");
            }
        }

        public void LoadBinary(string sourceFilename)
        {
            try
            {
                var ioService = ServiceHelper.GetIoService();
                FileName = sourceFilename;
                CurrentState= EditorState.Loaded;

                LatestTimeStamp = ioService.GetModificationTimeStamp(FileName);

                Task.Run(async () =>
                {
                    ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(true));

                    var byteContent = await ioService.ReadBinaryAsync(FileName);
                    var hexContent = ToHexStringRow(byteContent);

                    InitialText = hexContent;
                    textComponent.SetText(InitialText);
                    mainUi.SetFilename(FileName);

                    IsTextFormatAvailable = false;

                    ServiceHelper.TriggerFileUpdate();
                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("File is loaded as Binary", false));
                    ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
                });


            }
            catch (AggregateException aEx)
            {
                var exceptions = aEx.InnerExceptions;
                foreach (var inner in exceptions)
                {
                    ServiceHelper.ShowError(inner, "Load File");
                }

                ServiceHelper.TriggerStartStopAsnyOperation(new UpdateAsyncProcessState(false));
            }
            catch (Exception ex)
            {
                ServiceHelper.ShowError(ex, "Load File");
            }

        }

        private string ToHexStringRow(Memory<byte> byteContent)
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
                var rowHexCoding = Convert.ToHexString(bytesInRow.Span);

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
                var lastRowHexCoding = Convert.ToHexString(lastRow.Span);

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


        /// <summary>
        /// Handles the creation of new files
        /// </summary>
        public void New()
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

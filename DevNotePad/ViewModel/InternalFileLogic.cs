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
        private IMainViewUi ui;

        internal InternalFileLogic(IMainViewUi ui)
        {
            this.ui = ui;
            InitialText = String.Empty;
            LatestTimeStamp = DateTime.Now;
            FileName = "Unknown";
        }

        public string InitialText { get; set; }
        public DateTime LatestTimeStamp { get; set; }
        public string FileName { get; set; }

        public EditorState CurrentState { get; set; }

        public void InternalText(TextActionEnum textAction)
        {
            var isSelected = ui.IsTextSelected();
            if (!isSelected)
            {
                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("No Text selected. Please select a text first", true));
            }
            else
            {
                try
                {
                    var formatter = new TextFormatter();

                    bool isWarnung = false;
                    bool doUpdate = true;
                    string notifier;

                    var text = ui.GetText(true);
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
                        default:
                            formattedText = text;
                            isWarnung = true;
                            notifier = "Unknown Text action detected";
                            break;
                    }

                    ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter(notifier, isWarnung));

                    if (doUpdate)
                    {
                        ui.SetText(formattedText, true);
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
            ui.PerformClipboardAction(action);
        }

        /// <summary>
        /// Handles the internal save of the current Text and is called by Save and Save As
        /// </summary>
        /// <param name="filename">the filename</param>
        public bool InternalSave(string targetfilename)
        {
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
                    InitialText = ui.GetText(false);
                    ioService.WriteTextFile(targetfilename, InitialText);
                    CurrentState = EditorState.Saved;
                    LatestTimeStamp = DateTime.Now;

                    FileName = targetfilename;
                    ui.SetFilename(FileName);

                    isSuccessful = true;
                    ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Content is saved", false));

                    //State = "Saved";
                    //RaisePropertyChange("State");
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

        /// <summary>
        /// Handles the internal load of files and is called by ICommand delegates
        /// </summary>
        /// <param name="filename">the filename</param>
        public bool InternalLoad(string sourceFilename)
        {
            bool isSuccessful = false;

            try
            {
                var ioService = ServiceHelper.GetIoService();
                FileName = sourceFilename;
                CurrentState = EditorState.Loaded;

                //Store the timestamp of the file right now
                LatestTimeStamp = ioService.GetModificationTimeStamp(FileName);

                InitialText = ioService.ReadTextFile(FileName);
                ui.SetText(InitialText);
                ui.SetFilename(FileName);

                isSuccessful=true;
                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("File is loaded", false));

                //State = "Loaded";
                //RaisePropertyChange("State");
            }
            catch (Exception ex)
            {
                ServiceHelper.ShowError(ex, "Load File");
            }

            return isSuccessful;
        }

        /// <summary>
        /// Handles the creation of new files
        /// </summary>
        public bool InternalNew()
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

                ui.SetText(InitialText);
                ui.SetFilename(FileName);

                ServiceHelper.TriggerToolbarNotification(new UpdateStatusBarParameter("New file created", false));

                //State = "New";
                //RaisePropertyChange("State");
            }

            return proceed;
        }

        /// <summary>
        /// Handles the reload of a file
        /// </summary>
        public void InternalReload()
        {
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
                            InternalLoad(FileName);
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

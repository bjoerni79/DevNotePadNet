using DevNotePad.Features;
using DevNotePad.Features.JavaCardApplet;
using DevNotePad.MVVM;
using DevNotePad.Shared;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class AppletToolViewModel : MainViewUiViewModel
    {
        private const string Component = "Applet Tool";

        public AppletToolViewModel()
        {
            Select = new DefaultCommand(OnSelect);
            Load = new DefaultCommand(OnLoad);
            SaveToIjc = new DefaultCommand(OnSaveToIjc, IsAppletAvailable);
            SaveToHex = new DefaultCommand(OnSaveToHex, IsAppletAvailable);
            CreateReport = new DefaultCommand(OnCreateReport, IsAppletAvailable);
            Clear = new DefaultCommand(OnClear);

            FileName = String.Empty;
        }

        public DefaultCommand Clear { get; private set; }

        public DefaultCommand Select { get; private set; }

        public DefaultCommand Load { get; private set; }

        public DefaultCommand SaveToIjc { get; private set; }

        public DefaultCommand SaveToHex { get; private set; }

        public DefaultCommand CreateReport { get; private set; }

        public string FileName { get; set; }


        private IEnumerable<AppletComponent> RawComponents { get; set; }
        public ObservableCollection<AppletComponentViewItem> Components { get; set; }

        private AppletComponentViewItem currentComponent;
        public AppletComponentViewItem CurrentComponent
        {
            get
            {
                return currentComponent;
            }
            set
            {
                currentComponent = value;
                RaisePropertyChange("CurrentComponent");
            }
        }

        private bool IsAppletAvailable()
        {
            if (Components != null)
            {
                return Components.Any();
            }

            return false;
        }

        private void RefreshCommands()
        {
            Select.Refresh();
            Load.Refresh();
            SaveToIjc.Refresh();
            SaveToHex.Refresh();
            CreateReport.Refresh();

            RaisePropertyChange("Select");
            RaisePropertyChange("Load");
            RaisePropertyChange("SaveToIjc");
            RaisePropertyChange("SaveToHex");
            RaisePropertyChange("CreateReport");


        }

        private void OnClear()
        {
            FileName = String.Empty;
            Components = null;
            CurrentComponent = null;

            RaisePropertyChange("Components");
            RaisePropertyChange("CurrentComponent");
            RaisePropertyChange("FileName");
            RefreshCommands();
        }

        private void OnSelect()
        {
            var serviceDialog = ServiceHelper.GetDialogService();
            if (serviceDialog != null)
            {
                var currentWindow = dialog.GetCurrentWindow();
                var filename = serviceDialog.ShowOpenFileNameDialog("All | *.*| Text documents | *.txt | CAP Files | *.cap | IJC | *.ijc| Applet | *.hex;*.ijc;*.cap",currentWindow);
                if (filename.IsConfirmed)
                {
                    FileName = filename.File;
                    RaisePropertyChange("FileName");

                    // Load it now as well
                    OnLoad();
                }
            }

        }

        private void OnLoad()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                var currentWindow = dialog.GetCurrentWindow();
                ServiceHelper.ShowWarning("Please select a valid file first", Component, currentWindow);
            }
            else
            {
                if (!File.Exists(FileName))
                {
                    var currentWindow = dialog.GetCurrentWindow();
                    ServiceHelper.ShowWarning("Filename cannot be found", Component, currentWindow);
                }
                else
                {
                    var appletIo = FeatureFactory.CreateAppletIo();
                    var appletInterpreter = FeatureFactory.CreateAppletInterpeter();
                    try
                    {
                        RawComponents = appletIo.Read(FileName);

                        if (RawComponents.Any())
                        {
                            // Interpret them
                            var interpretedComponents = RawComponents.Select(c => appletInterpreter.Decode(c)).ToList();

                            //Convert them to UI instances
                            var viewItems = interpretedComponents.Select(c => new AppletComponentViewItem() { Id = c.ComponentId.ToString(), Content = c.InterpretedValue, Title = c.ComponentTitle }).ToList();
                            Components = new ObservableCollection<AppletComponentViewItem>(viewItems);
                            CurrentComponent = Components.First();

                            RaisePropertyChange("Components");
                            RaisePropertyChange("CurrentComponent");

                            RefreshCommands();
                        }
                    }
                    catch (Exception ex)
                    {
                        var currentWindow = dialog.GetCurrentWindow();
                        ServiceHelper.ShowError(ex, Component,currentWindow);
                    }
                }
            }
        }

        private string GetFilename(string defaultExtension)
        {
            string fileName = null;

            var serviceDialog = ServiceHelper.GetDialogService();
            if (serviceDialog != null)
            {
                var currentWindow = dialog.GetCurrentWindow();
                var fileNameResult = serviceDialog.ShowSaveFileDialog(defaultExtension,currentWindow);
                if (fileNameResult.IsConfirmed)
                {
                    fileName = fileNameResult.File;
                }
            }


            return fileName;
        }

        private void OnSaveToIjc()
        {
            var fileName = GetFilename("IJC|*.ijc");
            if (fileName != null && RawComponents != null && RawComponents.Any())
            {
                try
                {
                    var appletIo = FeatureFactory.CreateAppletIo();
                    appletIo.WriteAsByte(fileName, RawComponents);

                    //UpdateStatus(String.Format("File saved as {0}", fileName));
                }
                catch (Exception ex)
                {
                    var currentWindow = dialog.GetCurrentWindow();
                    ServiceHelper.ShowError(ex, Component, currentWindow);
                }

            }

        }

        private void OnSaveToHex()
        {
            var fileName = GetFilename("Text|*.txt");
            if (fileName != null && RawComponents != null && RawComponents.Any())
            {
                try
                {
                    var appletIo = FeatureFactory.CreateAppletIo();
                    appletIo.WriteAsHex(fileName, RawComponents);

                    //UpdateStatus(String.Format("File saved as {0}", fileName));
                }
                catch (Exception ex)
                {
                    var currentWindow = dialog.GetCurrentWindow();
                    ServiceHelper.ShowError(ex, Component,currentWindow);
                }

            }
        }

        private void OnCreateReport()
        {
            var fileName = GetFilename("Text|*.txt");
            if (fileName != null && RawComponents != null && RawComponents.Any())
            {
                // Export the interpreted applet components now.
                try
                {
                    using (var streamWriter = File.CreateText(fileName))
                    {
                        streamWriter.WriteLine("Date: " + DateTime.Now);
                        streamWriter.WriteLine("Applet: " + FileName);
                        streamWriter.WriteLine();

                        foreach (var component in Components)
                        {
                            streamWriter.WriteLine("+++ COMPONENT");
                            streamWriter.WriteLine("Tag = {0}, Description = {1}", component.Id, component.Title);
                            streamWriter.WriteLine("Content:");
                            streamWriter.WriteLine(component.Content);
                            streamWriter.WriteLine();
                        }

                        //UpdateStatus(String.Format("File saved as {0}", fileName));
                    }
                }
                catch (Exception ex)
                {
                    var currentWindow = dialog.GetCurrentWindow();
                    ServiceHelper.ShowError(ex, Component,currentWindow);
                }

            }
        }
    }
}

using CommunityToolkit.Mvvm.Input;
using DevNotePad.Features;
using DevNotePad.Features.Text;
using DevNotePad.MVVM;
using System;

namespace DevNotePad.ViewModel
{
    public class Base64ToolViewModel : MainViewUiViewModel
    {
        private const string DefaultExtension = "All|*.*|bin|*.bin";
        private const string ComponentName = "Base64";

        private ITextFormatComponent formatComponent;

        public Base64ToolViewModel()
        {
            ToHex = new RelayCommand(OnToHex);
            ToBase64 = new RelayCommand(OnToBase64);
            LoadBinary = new RelayCommand(OnLoadBinary);
            SaveBinary = new RelayCommand(OnSaveBinary);
            Close = new RelayCommand(OnClose);
            Reset = new RelayCommand(OnReset);

            formatComponent = FeatureFactory.CreateTextFormat();
        }

        public RelayCommand ToHex { get; private set; }

        public RelayCommand ToBase64 { get; private set; }

        public RelayCommand LoadBinary { get; private set; }

        public RelayCommand SaveBinary { get; set; }

        public RelayCommand Close { get; private set; }

        public RelayCommand Reset { get; private set; }

        public string? HexStringCoding { get; set; }

        public string? Base64StringCoding { get; set; }

        private void OnToHex()
        {
            var currentWindow = dialog.GetCurrentWindow();
            var dialogService = ServiceHelper.GetDialogService();

            if (!string.IsNullOrEmpty(Base64StringCoding))
            {
                try
                {
                    var groupedBase64 = formatComponent.GroupString(Base64StringCoding);
                    var byteCoding = Convert.FromBase64String(groupedBase64);
                    var hexCoding = Convert.ToHexString(byteCoding);

                    HexStringCoding = hexCoding;
                    OnPropertyChanged("HexStringCoding");
                }
                catch (FormatException formatException)
                {
                    dialogService.ShowWarningDialog(formatException.Message, ComponentName);
                }
            }
            else
            {
                dialogService.ShowWarningDialog("No Content available", ComponentName);
            }
        }

        private void OnToBase64()
        {
            var currentWindow = dialog.GetCurrentWindow();
            var dialogService = ServiceHelper.GetDialogService();

            if (!string.IsNullOrEmpty(HexStringCoding))
            {
                try
                {
                    var groupedHexString = formatComponent.GroupString(HexStringCoding);
                    var byteCoding = Convert.FromHexString(groupedHexString);

                    var base64Coding = Convert.ToBase64String(byteCoding);
                    Base64StringCoding = base64Coding;
                    OnPropertyChanged("Base64StringCoding");
                }
                catch (FormatException formatException)
                {
                    currentWindow = dialog.GetCurrentWindow();
                    dialogService = ServiceHelper.GetDialogService();

                    dialogService.ShowWarningDialog(formatException.Message, ComponentName);
                }
            }
            else
            {
                dialogService.ShowWarningDialog("No Content available", ComponentName);
            }
        }

        private void OnLoadBinary()
        {
            var dialogService = ServiceHelper.GetDialogService();

            var currentWindow = dialog.GetCurrentWindow();
            var returnValue = dialogService.ShowOpenFileNameDialog(DefaultExtension);
            if (returnValue.IsConfirmed)
            {
                var ioService = ServiceHelper.GetIoService();
                var fileName = returnValue.File;

                try
                {
                    //TODO: Asny?

                    var content = ioService.ReadBinary(fileName);
                    var hexContent = Convert.ToHexString(content);

                    HexStringCoding = hexContent;
                    OnPropertyChanged("HexStringCoding");
                }
                catch (Exception ex)
                {
                    dialogService.ShowErrorDialog(ex, ComponentName);
                }
            }
        }

        private void OnSaveBinary()
        {
            var dialogService = ServiceHelper.GetDialogService();

            var currentWindow = dialog.GetCurrentWindow();
            var returnValue = dialogService.ShowSaveFileDialog(DefaultExtension);

            if (!string.IsNullOrEmpty(HexStringCoding))
            {
                if (returnValue.IsConfirmed)
                {
                    var ioService = ServiceHelper.GetIoService();
                    var fileName = returnValue.File;

                    try
                    {
                        // Saves the content to a file. Shows a warning if the format is not valid

                        //TODO: Async?

                        var groupedHexString = formatComponent.GroupString(HexStringCoding);
                        var byteContent = Convert.FromHexString(groupedHexString);
                        ioService.WriteBinary(fileName, byteContent);
                    }
                    catch (FormatException formatException)
                    {
                        dialogService.ShowWarningDialog(formatException.Message, ComponentName);
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowErrorDialog(ex, "Base64");
                    }
                }
            }
            else
            {
                dialogService.ShowWarningDialog("No Content available", ComponentName);
            }

        }

        private void OnReset()
        {
            HexStringCoding = String.Empty;
            Base64StringCoding = String.Empty;

            OnPropertyChanged("HexStringCoding");
            OnPropertyChanged("Base64StringCoding");
        }

        private void OnClose()
        {
            dialog.CloseDialog(true);
        }
    }
}

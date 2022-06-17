﻿using DevNotePad.Features;
using DevNotePad.Features.Text;
using DevNotePad.MVVM;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevNotePad.ViewModel
{
    public class Base64ToolViewModel : MainViewUiViewModel
    {
        private const string DefaultExtension = "All|*.*|bin|*.bin";
        private const string ComponentName = "Base64";

        private ITextFormatComponent formatComponent;

        public Base64ToolViewModel()
        {
            ToHex = new DefaultCommand(OnToHex);
            ToBase64 = new DefaultCommand(OnToBase64);
            LoadBinary = new DefaultCommand(OnLoadBinary);
            SaveBinary = new DefaultCommand(OnSaveBinary);
            Close = new DefaultCommand(OnClose);
            Reset = new DefaultCommand(OnReset);

            formatComponent = FeatureFactory.CreateTextFormat();
        }

        public IRefreshCommand ToHex { get; private set; }

        public IRefreshCommand ToBase64 { get; private set; }

        public IRefreshCommand LoadBinary { get; private set; }

        public IRefreshCommand SaveBinary { get; set; }

        public ICommand Close { get; private set; }

        public ICommand Reset { get; private set; }

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
                    dialogService.ShowWarningDialog(formatException.Message, ComponentName, currentWindow);
                }
            }
            else
            {
                dialogService.ShowWarningDialog("No Content available", ComponentName, currentWindow);
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

                    dialogService.ShowWarningDialog(formatException.Message, ComponentName, currentWindow);
                }
            }
            else
            {
                dialogService.ShowWarningDialog("No Content available", ComponentName, currentWindow);
            }
        }

        private void OnLoadBinary()
        {
            var dialogService = ServiceHelper.GetDialogService();

            var currentWindow = dialog.GetCurrentWindow();
            var returnValue = dialogService.ShowOpenFileNameDialog(DefaultExtension, currentWindow);
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
                    dialogService.ShowErrorDialog(ex, ComponentName, currentWindow);
                }
            }
        }

        private void OnSaveBinary()
        {
            var dialogService = ServiceHelper.GetDialogService();

            var currentWindow = dialog.GetCurrentWindow();
            var returnValue = dialogService.ShowSaveFileDialog(DefaultExtension, currentWindow);

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
                        dialogService.ShowWarningDialog(formatException.Message, ComponentName, currentWindow);
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowErrorDialog(ex, "Base64", currentWindow);
                    }
                }
            }
            else
            {
                dialogService.ShowWarningDialog("No Content available", ComponentName, currentWindow);
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

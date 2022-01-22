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

        public Base64ToolViewModel()
        {
            ToHex = new DefaultCommand(OnToHex);
            ToBase64 = new DefaultCommand(OnToBase64);
            LoadBinary = new DefaultCommand(OnLoadBinary);
            SaveBinary = new DefaultCommand(OnSaveBinary);
            Close = new DefaultCommand(OnClose);
            Reset = new DefaultCommand(OnReset);
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
            //TODO: Review, Try..catch.. , check for null... Check for Non-HexStrings
            var byteCoding = Convert.FromBase64String(Base64StringCoding);
            var hexCoding = Convert.ToHexString(byteCoding);

            HexStringCoding = hexCoding;
            RaisePropertyChange("HexStringCoding");
        }

        private void OnToBase64()
        {
            //TODO: Review, Try..catch.. , check for null...Check for Non-HexStrings
            var byteCoding = Convert.FromHexString(HexStringCoding);

            var base64Coding = Convert.ToBase64String(byteCoding);
            Base64StringCoding = base64Coding;
            RaisePropertyChange("Base64StringCoding");
        }

        private void OnLoadBinary()
        {
            //TODO: Review, Try..catch.. , check for null...
            var dialogService = ServiceHelper.GetDialogService();

            var currentWindow = dialog.GetCurrentWindow();
            var returnValue = dialogService.ShowOpenFileNameDialog(DefaultExtension, currentWindow);
            if (returnValue.IsConfirmed)
            {
                var ioService = ServiceHelper.GetIoService();
                var fileName = returnValue.File;

                var content = ioService.ReadBinary(fileName);
                var hexContent = Convert.ToHexString(content);

                HexStringCoding = hexContent;
                RaisePropertyChange("HexStringCoding");
            }
        }

        private void OnSaveBinary()
        {
            //TODO: Review, Try..catch.. , check for null...

            var dialogService = ServiceHelper.GetDialogService();

            var currentWindow = dialog.GetCurrentWindow();
            var returnValue = dialogService.ShowSaveFileDialog(DefaultExtension, currentWindow);
            if (returnValue.IsConfirmed)
            {
                var ioService = ServiceHelper.GetIoService();
                var fileName = returnValue.File;

                var byteContent = Convert.FromHexString(HexStringCoding);
                ioService.WriteBinary(fileName, byteContent);
            }
        }

        private void OnReset()
        {
            HexStringCoding = String.Empty;
            Base64StringCoding = String.Empty;

            RaisePropertyChange("HexStringCoding");
            RaisePropertyChange("Base64StringCoding");
        }

        private void OnClose()
        {
            dialog.CloseDialog(true);
        }
    }
}

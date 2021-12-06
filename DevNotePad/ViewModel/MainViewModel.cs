using DevNotePad.MVVM;
using DevNotePad.Service;
using Generic.MVVM;
using Generic.MVVM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class MainViewModel : AbstractViewModel
    {
        public MainViewModel()
        {
            Text = "123\n456";
            FileName = "lala.txt";

            New = new DefaultCommand(OnNew);
            Open = new DefaultCommand(OnOpen);
            Save = new DefaultCommand(OnSave);
            SaveAs = new DefaultCommand(OnSaveAs);
        }

        public IRefreshCommand New { get; set; }

        public IRefreshCommand Open { get; set; }

        public IRefreshCommand Save { get; set; }

        public IRefreshCommand SaveAs { get; set; }


        public string Text { get; set; }

        public string FileName { get; set; }

        private void OnNew()
        {

        }

        private void OnOpen()
        {
            var dialogService = GetDialogService();
            var fileName = dialogService.ShowOpenFileNameDialog("Open New File", "*.txt", String.Empty);

            if (fileName != null)
            {
                var ioService = GetIoService();
                FileName = fileName;

                var text = ioService.ReadTextFile(FileName);
                Text = text;

                RaisePropertyChange("Text");
                RaisePropertyChange("FileName");
            }
        }

        private void OnSave()
        {

        }

        private void OnSaveAs()
        {

        }


    }
}

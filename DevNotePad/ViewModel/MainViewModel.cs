using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class MainViewModel : GenericViewModel
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

        }

        private void OnSave()
        {

        }

        private void OnSaveAs()
        {

        }
    }
}

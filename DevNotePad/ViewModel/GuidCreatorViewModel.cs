using CommunityToolkit.Mvvm.Input;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevNotePad.ViewModel
{
    public class GuidCreatorViewModel : MainViewUiViewModel
    {

        public GuidCreatorViewModel()
        {
            CreateGuid();

            Refresh = new RelayCommand(CreateGuid);
            Close = new RelayCommand(() => dialog.CloseDialog(true));
            CopyGuid1 = new RelayCommand(() => CopyGuid(1));
            CopyGuid2 = new RelayCommand(() => CopyGuid(2));
            CopyGuid3 = new RelayCommand(() => CopyGuid(3));
        }

        public string? Guid1 { get; set; }

        public string? Guid2 { get; set; }

        public string? Guid3 { get; set; }

        public string? Feedback { get; set; }

        public RelayCommand? Refresh { get; set; }

        public RelayCommand? Close { get; set; }

        public RelayCommand? CopyGuid1 { get; set; }

        public RelayCommand? CopyGuid2 { get; set; }

        public RelayCommand? CopyGuid3 { get; set; }

        private void CopyGuid(int id)
        {
            var guids = new [] { Guid1, Guid2, Guid3 };
            var selectedGuid = guids[id - 1];

            Feedback = String.Format("GUID {0} copied to clipboard", id);
            OnPropertyChanged("Feedback");

            Clipboard.SetText(selectedGuid);
        }

        private void CreateGuid()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            Guid guid3 = Guid.NewGuid();

            Guid1 = guid1.ToString();
            Guid2 = guid2.ToString();
            Guid3 = guid3.ToString();

            Feedback = "New GUIDs ready";
            OnPropertyChanged("Guid1");
            OnPropertyChanged("Guid2");
            OnPropertyChanged("Guid3");
            OnPropertyChanged("Feedback");
        }
    }
}

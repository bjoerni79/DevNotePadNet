using CommunityToolkit.Mvvm.Input;
using System;
using System.Text;
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
            CopyGuidAll = new RelayCommand(OnCopyGuidAll);
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

        public RelayCommand? CopyGuidAll { get; set; }

        private void CopyGuid(int id)
        {
            var guids = new[] { Guid1, Guid2, Guid3 };
            var selectedGuid = guids[id - 1];

            Feedback = String.Format("GUID {0} copied to clipboard", id);
            OnPropertyChanged("Feedback");

            Clipboard.SetText(selectedGuid);
        }

        private void OnCopyGuidAll()
        {
            var guids = new[] { Guid1, Guid2, Guid3 };

            var allGuids = new StringBuilder();
            int curId = 1;
            foreach (var curGuid in guids)
            {
                allGuids.AppendFormat("GUID {0} : {1}\n", curId, curGuid);
                curId++;
            }

            Feedback = String.Format("All GUIDs copied to clipboard");
            OnPropertyChanged("Feedback");

            Clipboard.SetText(allGuids.ToString());
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

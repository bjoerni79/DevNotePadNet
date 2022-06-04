﻿using DevNotePad.MVVM;
using DevNotePad.Shared;
using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace DevNotePad.ViewModel
{
    public class FindDialogViewModel : MainViewUiViewModel
    {
        private TextPointer? startIndex;
        private string? searchPattern;

        private SearchEngine searchEngine;

        public FindDialogViewModel()
        {
            Find = new DefaultCommand(OnFind);
            FindNext = new DefaultCommand(OnFindNext,() => startIndex != null);
            Cancel = new DefaultCommand(OnCancel);

            startIndex = null;
            searchEngine = new SearchEngine();
        }

        public bool IgnoreLetterType { get; set; }

        public bool StartFromCurrentPosition { get; set; }

        public string? SearchPattern
        {
            get
            {
                return searchPattern;
            }
            set
            {
                searchPattern = value;
            }
        }

        public IRefreshCommand Find { get; set; }

        public IRefreshCommand FindNext { get; set; }

        public IRefreshCommand Cancel { get; set; }

        private void OnFind()
        {
            // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/change-selection-in-a-richtextbox-programmatically?view=netframeworkdesktop-4.8

            if (StartFromCurrentPosition)
            {
                startIndex = textComponent.GetCurrentPosition();
            }
            else
            {
                startIndex = textComponent.GetStartPosition();
            }

            searchEngine.SearchPattern = SearchPattern;
            searchEngine.IgnoreLetterType = IgnoreLetterType;
            searchEngine.StartPosition = startIndex;

            FindNext.Refresh();
            OnFindNext();
        }

        private void OnFindNext()
        {
            var currentDocument = textComponent.GetDocument();
            var result = searchEngine.RunSearch(currentDocument);
            if (result.Successful)
            {
                textComponent.SelectText(result.Selection);
                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Found", false));
            }
            else
            {
                ServiceHelper.TriggerToolbarNotification(new Shared.Event.UpdateStatusBarParameter("Not found", true));
            }
        }

        private void OnCancel()
        {
            dialog.CloseDialog(true);
        }


    }
}

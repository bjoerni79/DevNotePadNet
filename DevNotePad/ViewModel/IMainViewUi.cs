﻿using DevNotePad.Shared.Event;

namespace DevNotePad.ViewModel
{
    public interface IMainViewUi
    {
        void SetScrollbars(bool enable);


        void ShowAbout();

        void SetFilename(string filename);

        void CloseByViewModel();

        void ResetLayout();
    }
}

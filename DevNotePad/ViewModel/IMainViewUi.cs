﻿using DevNotePad.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public interface IMainViewUi
    {
        void SetScrollbars(bool enable);
        void SetWordWrap(bool enable);

        void SetScratchPad(bool enable);

        void ShowAbout();

        void SetFilename(string filename);

        void CleanUpScratchPad();

        void CloseByViewModel();

        void ResetLayout();
    }
}

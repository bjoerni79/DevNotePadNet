﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class ReplaceDialogViewModel : AbstractViewModel
    {
        private IMainViewUi ui;

        public ReplaceDialogViewModel(IMainViewUi mainViewUi)
        {
            ui = mainViewUi;
        }


    }
}

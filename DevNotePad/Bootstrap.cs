using DevNotePad.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad
{
    public class Bootstrap
    {
        public const string BootstrapId = "bootstrap";

        public Bootstrap()
        {
            FacadeFactory.InitFactory();

            //TODO: Init Main ViewModel
        }

        public void Init()
        {
            //TODO
        }
    }
}

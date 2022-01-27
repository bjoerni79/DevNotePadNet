using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.JavaCardApplet
{
    public interface IAppletIO
    {
        IEnumerable<AppletComponent> Read(string file);
        //IEnumerable<AppletComponent> Read(... stream ...);

        void WriteAsByte(string file, IEnumerable<AppletComponent> components);
        void WriteAsHex(string file, IEnumerable<AppletComponent> components);
    }
}

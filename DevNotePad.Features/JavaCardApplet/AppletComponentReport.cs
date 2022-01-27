using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.JavaCardApplet
{
    public class AppletComponentReport
    {
        public AppletComponentReport()
        {
            ComponentTitle = "Unknown";
            RawValue = new List<byte>();
            InterpretedValue = "None";
        }

        public int ComponentId { get; set; }

        public string ComponentTitle { get; set; }

        public IEnumerable<byte> RawValue { get; set; }

        public string InterpretedValue { get; set; }

    }
}

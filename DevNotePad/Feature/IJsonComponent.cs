using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Feature
{
    public interface IJsonComponent
    {
        string Formatter(string jsonText);

        string Parse(string jsonText);
    }
}

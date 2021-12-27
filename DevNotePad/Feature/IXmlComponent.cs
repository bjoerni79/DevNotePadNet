using DevNotePad.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Feature
{
    public interface IXmlComponent
    {
        string Formatter(string xmlText);

        string ParseToString(string xmlText);

        ItemNode ParseToTree(string xmlText);
    }
}

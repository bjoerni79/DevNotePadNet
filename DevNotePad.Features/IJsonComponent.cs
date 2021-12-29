using DevNotePad.Features.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features
{
    public interface IJsonComponent
    {
        string Formatter(string jsonText);

        string ParseToString(string jsonText);

        ItemNode ParseToTree(string jsonText);
    }
}

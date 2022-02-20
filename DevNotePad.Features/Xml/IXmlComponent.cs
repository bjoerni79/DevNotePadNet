using DevNotePad.Features.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Xml
{
    public interface IXmlComponent
    {
        string Formatter(string xmlText);

        string ParseToString(string xmlText);

        IEnumerable<ItemNode> ParseToTree(string xmlText);

        Task<IEnumerable<ItemNode>> ParseToTreeAsync(string xmlText);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Xml
{

    public record SchemaCompareResult (bool IsPassed);

    public record SchemaCompareRequest(string XmlContent);
}

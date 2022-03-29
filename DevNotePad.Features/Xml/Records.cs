using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Xml
{
    public record SchemaValidationItem (string Category, string Description);

    public record SchemaCompareResult (bool IsPassed,IEnumerable<SchemaValidationItem>? ValidationItems);

    public record SchemaCompareRequest(TextReader XmlContent, StreamReader Schema);

    public record XPathQueryRequest (TextReader xmlContent, string XPathQuery);

    public record XPathQueryResponse (bool IsMatching, string result);
}

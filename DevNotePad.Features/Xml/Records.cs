using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DevNotePad.Features.Xml
{
    public record SchemaValidationItem (string Category, string Description);

    public record SchemaCompareResult (bool IsPassed,IEnumerable<SchemaValidationItem>? ValidationItems);

    public record SchemaCompareRequest(TextReader XmlContent, StreamReader Schema);

    public record XPathQueryRequest (TextReader XmlContent, string XPathQuery);

    public record XPathQueryResponse (bool IsMatching, string Result);

    public record XSltTransformationRequest(TextReader XmlContent, XmlReader Transformation);

    public record XSltTransformationResponse (bool IsPassed, string Result);
}

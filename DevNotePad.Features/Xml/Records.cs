using System.Xml;

namespace DevNotePad.Features.Xml
{
    public record SchemaValidationItem(string Category, string Description);

    public record SchemaCompareResult(bool IsPassed, IEnumerable<SchemaValidationItem>? ValidationItems);

    public record SchemaCompareRequest(TextReader XmlContent, StreamReader Schema);

    public record XPathQueryRequest(TextReader XmlContent, string XPathQuery);

    public record XPathQueryResponse(bool IsMatching, string Result);

    public record XsltParameter(bool EnableScript, bool EnableDocumentFunction);

    public record XSltTransformationRequest(TextReader XmlContent, XmlReader Transformation, XsltParameter Parameter);

    public record XSltTransformationResponse(bool IsPassed, string Result);
}

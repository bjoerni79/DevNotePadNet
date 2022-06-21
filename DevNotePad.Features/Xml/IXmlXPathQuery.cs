namespace DevNotePad.Features.Xml
{
    public interface IXmlXPathQuery
    {
        XPathQueryResponse Query(XPathQueryRequest request);
    }
}

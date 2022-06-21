namespace DevNotePad.Features.Xml
{
    public interface IXmlXsltTransformer
    {
        //TODO
        // See https://docs.microsoft.com/en-us/dotnet/standard/data/xml/using-the-xslcompiledtransform-class

        XSltTransformationResponse Transform(XSltTransformationRequest request);
    }
}

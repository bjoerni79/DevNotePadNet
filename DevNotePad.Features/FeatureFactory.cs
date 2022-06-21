using DevNotePad.Features.Json;
using DevNotePad.Features.Text;
using DevNotePad.Features.TlvDecoder;
using DevNotePad.Features.Xml;

namespace DevNotePad.Features
{
    public static class FeatureFactory
    {
        public static IXmlXsltTransformer CreateXsltTransformer()
        {
            return new XmlXsltTransformer();
        }

        public static IXmlXPathQuery CreateXmlXpathQuery()
        {
            return new XmlXPathQuery();
        }

        public static IXmlSchemaValidator CreateXmlSchemaValidator()
        {
            return new XmlSchemaValidator();
        }
        public static IXmlComponent CreateXml()
        {
            return new XmlComponent();
        }

        public static IJsonComponent CreateJson()
        {
            return new JsonComponentOld();
        }

        public static ITextFormatComponent CreateTextFormat()
        {
            return new TextFormatComponent();
        }

        public static ITlvDecoder CreateTlvDecoder()
        {
            return new TlvDecoder.TlvDecoder();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace DevNotePad.Features.Xml
{
    internal class XmlXsltTransformer : IXmlXsltTransformer
    {
        internal XmlXsltTransformer()
        {

        }

        public XSltTransformationResponse Transform(XSltTransformationRequest request)
        {
            // XSLT classes : https://docs.microsoft.com/en-us/dotnet/standard/data/xml/inputs-to-the-xslcompiledtransform-class

            var xmlContentReader = request.XmlContent;
            var transformationReader = request.Transformation;

            if (xmlContentReader == null)
            {
                throw new ArgumentException("XML Content is not available");
            }

            if (transformationReader == null)
            {
                throw new ArgumentException("XML Transformation stream is not available");
            }

            // Read the XSLT file

            var xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            var buffer = new StringBuilder();
            var isPassed = false;
            var result = String.Empty;

            try
            {
                // Load the Transformation content
                var xslTransform = new XslCompiledTransform();
                xslTransform.Load(transformationReader);

                // And now now parse the source using the XPathDocument and store it in a buffer via XmlWriter
                var document = new XPathDocument(xmlContentReader);
                using (var writer = XmlWriter.Create(buffer,xmlWriterSettings))
                {
                    xslTransform.Transform(document, writer);
                    isPassed = true;
                    result = buffer.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new FeatureException(ex.Message);
            }

            return new XSltTransformationResponse(isPassed,result);
        }
    }
}

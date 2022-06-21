using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace DevNotePad.Features.Xml
{
    internal class XmlXPathQuery : IXmlXPathQuery
    {
        internal XmlXPathQuery()
        {

        }

        public XPathQueryResponse Query(XPathQueryRequest request)
        {
            // see XPathDocument class : https://docs.microsoft.com/en-us/dotnet/api/system.xml.xpath.xpathdocument?view=net-6.0
            // and also: https://docs.microsoft.com/en-us/dotnet/api/system.xml.xpath.xpathnavigator?view=net-6.0

            var textReader = request.XmlContent;
            var rawQuery = request.XPathQuery;

            if (textReader == null)
            {
                throw new ArgumentException("Text Reaader instance for the XML content is null");
            }

            if (rawQuery == null)
            {
                throw new ArgumentException("XPath query is null");
            }

            bool isMatching = false;
            string queryResult = String.Empty;

            try
            {
                // Creates a XPath document that provides a XPathNavigator instance
                var xpathDocument = new XPathDocument(textReader);
                var navigator = xpathDocument.CreateNavigator();

                // Compile the query and evaluate it.
                var compiledQuery = navigator.Compile(rawQuery);
                var evalulationResult = navigator.Evaluate(compiledQuery);

                if (evalulationResult != null)
                {
                    var nodeIterator = evalulationResult as XPathNodeIterator;
                    isMatching = true;

                    if (nodeIterator != null)
                    {
                        // Iterate...
                        var iteraterBuffer = new StringBuilder("Detected Node(s) :\n");
                        while (nodeIterator.MoveNext())
                        {
                            iteraterBuffer.AppendFormat("- {0} at Position {1}\n", nodeIterator.Current.LocalName, nodeIterator.CurrentPosition);
                        }

                        queryResult = iteraterBuffer.ToString();
                    }
                    else
                    {
                        // Single element. Return as ToString()
                        queryResult = evalulationResult.ToString();
                    }
                }
            }
            catch (XmlException xmlEx)
            {
                throw new FeatureException("XML issue", xmlEx);
            }
            catch (Exception ex)
            {
                throw new FeatureException("Unknown issue", ex);
            }

            return new XPathQueryResponse(isMatching, queryResult);
        }
    }
}

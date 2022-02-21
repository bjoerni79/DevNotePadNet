using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Xml
{
    internal class XmlSchemaValidator : IXmlSchemaValidator
    {
        internal XmlSchemaValidator()
        {
        }

        public Task<SchemaCompareResult> CompareAsync(SchemaCompareRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

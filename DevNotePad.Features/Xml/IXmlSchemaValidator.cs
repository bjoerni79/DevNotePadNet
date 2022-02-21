﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Xml
{
    public interface IXmlSchemaValidator
    {
        Task<SchemaCompareResult> CompareAsync (SchemaCompareRequest request);
    }
}

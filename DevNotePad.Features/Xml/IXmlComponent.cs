﻿using DevNotePad.Features.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Xml
{
    public interface IXmlComponent
 {
        Task<string> FormatterAsync(string xmlText);

        Task<IEnumerable<ItemNode>> ParseToTreeAsync(string xmlText);
    }
}

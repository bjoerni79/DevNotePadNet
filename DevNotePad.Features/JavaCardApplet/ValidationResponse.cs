using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.JavaCardApplet
{
    internal class ValidationResponse
    {
        internal ValidationResponse()
        {
            IsValid = true;
            ValidationError = String.Empty;
        }

        internal bool IsValid { get; set; }

        internal string ValidationError { get; set; }
    }
}

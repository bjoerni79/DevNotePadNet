using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features
{
    public class FeatureException : ApplicationException
    {
        public FeatureException(string message) : base(message)
        {

        }

        public FeatureException(string message, Exception inner) : base(message,inner)
        {
            Details = inner.Message;
        }

        public string Details { get; set; }
    }
}

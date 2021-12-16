using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Feature
{
    internal class JsonComponent : IJsonComponent
    {
        internal JsonComponent()
        {

        }

        public string Formatter(string jsonText)
        {
            return jsonText;
        }
    }
}

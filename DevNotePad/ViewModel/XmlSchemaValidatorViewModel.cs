using Generic.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public class XmlSchemaValidatorViewModel : MainViewUiViewModel
    {
        public XmlSchemaValidatorViewModel()
        {

        }

        public string? SchemaFile { get; set; }

        public string? XmlContent { get; set; }


        public IRefreshCommand? ImportFromText { get; private set; }

        public IRefreshCommand? ExportToText { get; private set; }

        public IRefreshCommand? ReadFromFile { get; private set; }

        public IRefreshCommand? Valdate { get; private set; }

        public IRefreshCommand? Clear { get; private set; }


        /*
         * see also : https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-6.0#xmlreader_nodes
         * via IXmlSchemaValidator interface
         * 
         * 
         * - Import from text
         * - Export to text
         * - Read from text file
         * 
         * - Import schema file
         * - Import
         * 
         * - Log? 
         * 
         * - Clear All
         */
    }
}

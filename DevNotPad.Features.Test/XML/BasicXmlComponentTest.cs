using DevNotePad.Features;
using DevNotePad.Features.Shared;
using DevNotePad.Features.Xml;
using DevNotPad.Features.Test.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotPad.Features.Test.XML
{
    [TestClass]
    public class BasicXmlComponentTest
    {
        private IXmlComponent? xmlComponentUt;

        [TestInitialize]
        public void Init()
        {
            xmlComponentUt = FeatureFactory.CreateXml(); 

        }

        /*
         * 
         *  - XmlDeclaration
         *   - Whitespace
         *   - Element Menu
         *   - Whitespace
         *   - Element Popup
         *   - Whitespace
         *   - Element MenuItem
         *   - Whitespace
         *   - Element MenuItem
         *   - Whitespace
         *   - Element MenuItem
         *   - Whitespace
         *   - EndElement
         *   - Whitespace
         *   - EndElement 
         * 
         *      <?xml version="1.0" encoding="utf-8"?>
                <menu id="file" value="File">
                  <popup>
                    <menuitem value="New" onclick="CreateNewDoc()" />
                    <menuitem value="Open" onclick="OpenDoc()" />
                    <menuitem value="Close" onclick="CloseDoc()" />
                  </popup>
                </menu>
         * 
         */

        [TestMethod]
        public void PlaygroundTest()
        {
            var xmlcontent = Resources.simpleXml1;
            
            var itemNodes = xmlComponentUt!.ParseToTree(xmlcontent);

            // Check the tree now
            Assert.IsNotNull(itemNodes);

            var xmlDeclare = itemNodes.First();
            Assert.AreEqual(xmlDeclare.Style, ItemNodeStyle.Meta);

            var element1 = itemNodes.Skip(1).First();
            Assert.AreEqual(element1.Style, ItemNodeStyle.Element);

        }
    }
}

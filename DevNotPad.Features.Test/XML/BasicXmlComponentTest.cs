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
        private IXmlComponent? xmlComponent;
        private IXmlComponent? xmlComponentUt;

        [TestInitialize]
        public void Init()
        {
            xmlComponent = FeatureFactory.CreateXml();
            xmlComponentUt = FeatureFactory.CreateXmlUnderTest(); 

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
         */

        [TestMethod]
        public void PlaygroundTest()
        {
            var xmlcontent = Resources.simpleXml1;
            
            var tree = xmlComponent!.ParseToTree(xmlcontent);
            var treeUt = xmlComponentUt!.ParseToTree(xmlcontent);

            // Check the tree now
            Assert.IsNotNull(tree);
            Assert.AreEqual(tree.Style, ItemNodeStyle.Title);

            Assert.IsNotNull(treeUt);
            Assert.AreEqual(treeUt.Style, ItemNodeStyle.Title);
        }
    }
}

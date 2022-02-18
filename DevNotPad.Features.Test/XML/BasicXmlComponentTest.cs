using DevNotePad.Features;
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
        private IXmlComponent xmlComponent;

        [TestInitialize]
        public void Init()
        {
            xmlComponent = FeatureFactory.CreateXml();
        }

        [TestMethod]
        public void PlaygroundTest()
        {
            var xmlcontent = Resources.simpleXml1;
            var tree = xmlComponent.ParseToTree(xmlcontent);

            // Check the tree now
            Assert.Fail("Todo");
        }
    }
}

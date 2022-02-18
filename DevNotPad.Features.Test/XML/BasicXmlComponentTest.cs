using DevNotePad.Features;
using DevNotePad.Features.Xml;
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

        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features
{
    public static class FeatureFactory
    {

        public static IXmlComponent CreateXml()
        {
            return new XmlComponent();
        }

        public static IJsonComponent CreateJson()
        {
            return new JsonComponent();
        }


    }
}
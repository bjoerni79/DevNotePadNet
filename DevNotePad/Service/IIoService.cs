﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Service
{
    public interface IIoService
    {

        string ReadTextFile(string filename);

        void WriteTextFile(string filename, string text);

        DateTime GetModificationTimeStamp(string filename);
    }
}

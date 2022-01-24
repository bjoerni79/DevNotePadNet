﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Service
{
    public interface IIoService
    {
        bool ExistFile(string path);

        string ReadTextFile(string filename);

        Task<string> ReadTextFileAsync(string filename);

        Span<byte> ReadBinary(string filename);

        //Task<Memory<byte>> ReadBinaryAsync(string filename);

        void WriteTextFile(string filename, string text);

        void WriteBinary(string filename, Span<byte> content);

        DateTime GetModificationTimeStamp(string filename);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.JavaCardApplet
{
    public class AppletComponent
    {
        //  U1 = Tag
        //  U2 = Size
        //  UL = Data [1...size]

        public AppletComponent()
        {
            Data = new List<byte>();
        }

        public AppletComponent(int tag, int size, IEnumerable<byte> value)
        {
            Tag = tag;
            Size = size;
            Data = value;
        }

        public int Tag { get; private set; }
        public int Size { get; private set; }
        public IEnumerable<byte> Data { get; private set; }

        /// <summary>
        /// Extracts the component from the stream
        /// </summary>
        public void Read(Stream inputStream)
        {
            // The structure is
            // Component:
            //  - U1 Tag
            //  - U2 Size
            //  - UL info[]
            //
            // See also chapter 6.1 of JCVM Spec

            if (inputStream.CanRead)
            {
                using (var zipStream = inputStream)
                {
                    // Read the Tag
                    int tag = zipStream.ReadByte();

                    // Read the size
                    byte[] u2Buffer = new byte[2];
                    zipStream.Read(u2Buffer, 0, 2);
                    int size = u2Buffer[0];
                    size <<= 8;
                    size += u2Buffer[1];

                    // Read the Info
                    byte[] info = new byte[size];
                    zipStream.Read(info, 0, size);

                    Tag = tag;
                    Size = size;
                    Data = info;
                }
            }
        }
    }
}

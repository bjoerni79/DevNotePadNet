using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.TlvDecoder
{
    /// <summary>
    /// Represents the TLV coding 
    /// </summary>
    public class Tlv
    {
        /// <summary>
        /// Creates a new TLV coding instance with default values
        /// </summary>
        public Tlv()
        {
            IsDefinite = true;
            TagBytes = new byte[0];
            LengthBytes = new byte[0];
            ByteValue = new byte[0];
        }

        /// <summary>
        /// The tag 
        /// </summary>
        public int Tag { get; internal set; }

        public byte[] TagBytes { get; internal set; }

        /// <summary>
        /// The Length
        /// </summary>
        public int Length { get; internal set; }

        public byte[] LengthBytes { get; internal set; }

        /// <summary>
        /// The Value as byte
        /// </summary>
        public byte[] ByteValue { get; internal set; }

        public byte[]? RemainingBytes { get; internal set; }

        public bool IsDefinite { get; set; }
    }
}

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
            Childs = new Tlv[0];
            ByteValue = new byte[0];
            RemainingBytes = new byte[0];
            Meaning = String.Empty;
            IsDefinite = true;
        }

        /// <summary>
        /// The child TLVs
        /// </summary>
        public Tlv[] Childs { get; internal set; }
        /// <summary>
        /// The tag 
        /// </summary>
        public int Tag { get; internal set; }
        /// <summary>
        /// The Length
        /// </summary>
        public int Length { get; internal set; }
        /// <summary>
        /// The Value as byte
        /// </summary>
        public byte[] ByteValue { get; internal set; }
        /// <summary>
        /// Field for meaning
        /// </summary>
        public string Meaning { get; internal set; }
        /// <summary>
        /// The remaining bytes left in the stream. Can be ignored if the TLV is already parsed
        /// </summary>
        public byte[] RemainingBytes { get; internal set; }

        public bool IsDefinite { get; set; }
    }
}

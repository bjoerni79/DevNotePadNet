using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.TlvDecoder
{
    internal class TlvDecoder : ITlvDecoder
    {
        private const byte BER_TLV_Pattern = 0x1F;

        internal TlvDecoder()
        {

        }

        public Tlv Decode(IEnumerable<byte> tlvCoding)
        {
            if (tlvCoding == null)
            {
                throw new ArgumentNullException("tlvCoding");
            }

            if (!tlvCoding.Any())
            {
                throw new ArgumentException("no TLV bytes detected");
            }

            var rootTlv = Parse(tlvCoding);
            return rootTlv;
        }

        private Tlv Parse(IEnumerable<byte> tlvCoding)
        {
            if (tlvCoding.Count() < 2)
            {
                throw new ArgumentException("Two bytes are required at least", "tlvBytes");
            }
            var tlv = new Tlv();
            var workingBuffer = tlvCoding.ToArray();

            // Read the Tag, Length and Value now. Any remaining bytes are stored later.
            workingBuffer = ReadTag(workingBuffer, tlv);
            workingBuffer = ReadLength(workingBuffer, tlv);
            workingBuffer = ReadValue(workingBuffer, tlv);
            if (workingBuffer.Any())
            {
                tlv.RemainingBytes = workingBuffer.ToArray();
            }

            return tlv;
        }

        private byte[] ReadTag(byte[] tlvBytes, Tlv tlv)
        {
            //
            // See ISO 8825-1 for all codings
            //

            int tagLength;
            var firstTagByte = tlvBytes.First();

            // Check if the the TLV is a BER-TLV or not
            // xxx1 1111 (0x1F) is reserverd for BER.
            var isBER = (firstTagByte & BER_TLV_Pattern) == BER_TLV_Pattern;
            if (isBER)
            {
                tagLength = 2;
                // x x y 1 1 1 1 1
                // xx = Universal (00) Application (01) Context spec. (10) Private (11)
                // y  = Primitve (0), Constructed (1) 

                // Get the next byte
                var secondTagByte = tlvBytes.Skip(1).First();

                var areMoreBytesAvailable = (secondTagByte & 0x80) == 1;
                if (areMoreBytesAvailable)
                {
                    throw new FeatureException("The TLV decoder only supports BER-TLV codings with one addional byte (7F 21 i.e at the moment)");
                }
                else
                {
                    int tagBuffer = firstTagByte << 8;
                    tagBuffer += secondTagByte;

                    tlv.Tag = tagBuffer;
                }

                tlv.TagBytes = new byte[] { firstTagByte, secondTagByte };
            }
            else
            {
                tagLength = 1;
                tlv.Tag = firstTagByte;
                tlv.TagBytes = new byte[] { firstTagByte };
            }



            return tlvBytes.Skip(tagLength).ToArray();
        }

        private byte[] ReadLength(byte[] tlvBytes, Tlv tlv)
        {
            byte[] returnBytes;
            var firstLengthByte = tlvBytes.First();

            // 0x80 = Indefinite form
            // others = Definite form


            if (firstLengthByte == 0x80)
            {
                // Indefinite length coding

                tlv.IsDefinite = false;
                tlv.Length = 0x80;
                tlv.LengthBytes = new byte[] { firstLengthByte };

                returnBytes = tlvBytes.Skip(1).ToArray();
            }
            else
            {
                // Definite length coding
                tlv.IsDefinite = true;
                if (firstLengthByte <= 0x7F)
                {
                    // Only one Length byte
                    tlv.Length = firstLengthByte;
                    tlv.LengthBytes = new byte[] { firstLengthByte };

                    returnBytes = tlvBytes.Skip(1).ToArray();
                }
                else
                {
                    int lengthCount = 0;
                    if (firstLengthByte == 0x81)
                    {
                        lengthCount = 1;
                    }

                    if (firstLengthByte == 0x82)
                    {
                        lengthCount = 2;
                    }

                    if (firstLengthByte == 0x83)
                    {
                        lengthCount = 3;
                    }

                    var bytes = tlvBytes.Skip(1).Take(lengthCount).ToArray();
                    int lengthBuffer = 0;
                    if (lengthCount == 1)
                    {
                        lengthBuffer = bytes[0];
                    }
                    if (lengthCount == 2)
                    {
                        lengthBuffer = bytes[0];
                        lengthBuffer <<= 8;
                        lengthBuffer = lengthBuffer + bytes[1];
                    }
                    if (lengthBuffer == 3)
                    {
                        // Not tested!
                        lengthBuffer = bytes[0];
                        lengthBuffer <<= 8;
                        lengthBuffer = lengthBuffer + bytes[1];
                        lengthBuffer <<= 8;
                        lengthBuffer = lengthBuffer + bytes[2];
                    }

                    tlv.Length = lengthBuffer;
                    tlv.LengthBytes = bytes;

                    returnBytes = tlvBytes.Skip(1).Skip(lengthCount).ToArray();
                }
            }

            return returnBytes;
        }

        private byte[] ReadValue(byte[] tlvBytes, Tlv tlv)
        {
            byte[] returnBytes;

            if (tlv.IsDefinite)
            {
                var length = tlv.Length;
                var byteCount = tlvBytes.Length;

                // The length must be equal or lesser than the available bytes. Otherwise the coding is invalid.
                if (length > byteCount)
                {
                    throw new ArgumentException(String.Format("TLV with tag {0:X} has an invalid length coding", tlv.Tag));
                }

                tlv.ByteValue = tlvBytes.Take(length).ToArray();
                returnBytes = tlvBytes.Skip(length).ToArray();
            }
            else
            {
                // Indefinite length coding. Read the until "00 00" terminates the TLV.
                var lastTwoBytes = tlvBytes.TakeLast(2).ToArray();
                if (lastTwoBytes[0] == 0x00 && lastTwoBytes[1] == 0x00)
                {
                    int length = tlvBytes.Length - 2;
                    tlv.ByteValue = tlvBytes.Take(length).ToArray();
                    return new byte[0];
                }
                else
                {
                    string error = String.Format("TLV with tag={0:X} and indefinite coding does not has any termination bytes (00 00)", tlv.Tag);
                    throw new FeatureException(error);
                }
            }

            return returnBytes;
        }

    }
}

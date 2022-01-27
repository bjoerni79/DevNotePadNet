using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.Shared
{
    public class HexConverter
    {
        public HexConverter()
        {

        }

        public string ConvertToHexString(IEnumerable<byte> bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            var byteArray = bytes.ToArray();

            var sb = new StringBuilder();
            var hexStringList = byteArray.Select(b => String.Format("{0:X2}", b));
            foreach (var hex in hexStringList)
            {
                sb.Append(hex);
            }

            return sb.ToString();
        }

        public string ConvertFromSpanToHexString(Span<byte> span)
        {
            var array = span.ToArray();
            return ConvertToHexString(array);
        }

        public byte[] ConvertToByteArray(string hexString)
        {
            if (hexString == null)
            {
                throw new ArgumentNullException("hexString");
            }

            int length = hexString.Length;
            // Verify first if the hex string is valid
            if (length % 2 != 0)
            {
                throw new ArgumentException("Hexstring is not a valid: There must be pair of 2 digits each");
            }

            // Convert the bytes
            var upperCaseCoding = hexString.ToUpper();
            var bytes = new List<byte>();
            for (int i=0; i< length; i+=2)
            {
                var hexByte = upperCaseCoding.Substring(i, 2);
                var isValidHexByte = hexByte.All(c =>  Char.IsDigit(c) || c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F');

                if (isValidHexByte)
                {
                    var byteValue = Byte.Parse(hexByte, NumberStyles.HexNumber);
                    bytes.Add(byteValue);
                }
                else
                {
                    throw new ArgumentException("Hexstring is not valid: Only characters from A-F and numbers are allowed.");
                }

            }

            return bytes.ToArray();
        }
    }
}

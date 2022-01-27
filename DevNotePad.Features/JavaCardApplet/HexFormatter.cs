using DevNotePad.Features.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.JavaCardApplet
{
    internal class HexFormatter 
    {
        HexConverter converter = new HexConverter();

        internal HexFormatter()
        {
            
        }

        public byte[] Convert(string hexString)
        {
            return converter.ConvertToByteArray(hexString);
        }

        public string Convert(IEnumerable<byte> bytes)
        {
            return converter.ConvertToHexString(bytes);
        }

        public string PrettyPrint(string hexString)
        {
            var response = Validate(hexString);
            if (response.IsValid)
            {
                var bytes = Convert(hexString);
                return PrettyPrint(bytes);
            }

            throw new FeatureException("Could not pretty print hex string. Invalid?");
        }

        public string PrettyPrint(IEnumerable<byte> bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var curByte in bytes)
            {
                sb.AppendFormat("{0:X2} ", curByte);
            }

            return sb.ToString();
        }

        public ValidationResponse Validate(string hexString)
        {
            var response = new ValidationResponse();

            if (String.IsNullOrEmpty(hexString))
            {
                response.IsValid = false;
                response.ValidationError = "hexString is null or empty";
            }

            if (response.IsValid)
            {
                int length = hexString.Length;
                // Verify first if the hex string is valid
                if (length % 2 != 0)
                {
                    response.IsValid = false;
                    response.ValidationError = "Hexstring is not a valid: There must be pair of 2 digits each";
                }
            }

            if (response.IsValid)
            {
                int length = hexString.Length;

                // Convert the bytes
                var upperCaseCoding = hexString.ToUpper();
                for (int i = 0; i < length; i += 2)
                {
                    var hexByte = upperCaseCoding.Substring(i, 2);
                    var isValidHexByte = hexByte.All(c => Char.IsDigit(c) || c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F');

                    if (!isValidHexByte)
                    {
                        response.IsValid = false;
                        response.ValidationError = "Hexstring is not valid: Only characters from A-F and numbers are allowed.";
                    }

                }
            }

            return response;
        }
    }
}

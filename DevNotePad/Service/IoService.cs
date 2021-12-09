using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Service
{
    public class IoService : IIoService
    {
        public IoService()
        {

        }

        public string ReadTextFile(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("filename is null or empty", filename);
            }

            // throw new FileNotFoundException("File cannot be found", filename);
            var fileExists = File.Exists(filename);
            if (!fileExists)
            {
                throw new FileNotFoundException("File cannot be found", filename);
            }

            return File.ReadAllText(filename);
        }

        public void WriteTextFile(string filename, string text)
        {
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("filename is null or empty", filename);
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            using (var stream = File.CreateText(filename))
            {
                stream.Write(text);
            }
        }
    }
}

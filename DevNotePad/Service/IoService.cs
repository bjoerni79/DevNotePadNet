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

        public bool ExistFile(string filename)
        {
            var fileExists = File.Exists(filename);
            return fileExists;
        }

        public DateTime GetModificationTimeStamp(string filename)
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

            var date = File.GetLastWriteTime(filename);
            return date;
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

        public async Task<string> ReadTextFileAsync(string filename)
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

            string content = await File.ReadAllTextAsync(filename);
            return content;
        }

        public Span<byte> ReadBinary(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("filename is null or empty", filename);
            }

            var fileExists = File.Exists(filename);
            if (!fileExists)
            {
                throw new FileNotFoundException("File cannot be found", filename);
            }

            // Read the bytes
            byte[] content;
            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                var length = stream.Length;
                content = new byte[length];
                stream.Read(content);
            }

            return content;
        }

        //public Task<Memory<byte>> ReadBinaryAsync(string filename)
        //{
        //    if (String.IsNullOrEmpty(filename))
        //    {
        //        throw new ArgumentException("filename is null or empty", filename);
        //    }

        //    var fileExists = File.Exists(filename);
        //    if (!fileExists)
        //    {
        //        throw new FileNotFoundException("File cannot be found", filename);
        //    }

        //    //// Read the bytes
        //    //byte[] content;
        //    //using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
        //    //{
        //    //    var length = stream.Length;
        //    //    content = new byte[length];
        //    //    stream.Read(content);
        //    //}

        //    //return content;
        //}

        public void WriteBinary(string filename, Span<byte> content)
        {
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("filename is null or empty", filename);
            }

            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            using (var stream = File.Create(filename))
            {
                stream.Write(content);
            }
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

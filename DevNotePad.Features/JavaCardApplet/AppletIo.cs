using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.JavaCardApplet
{
    internal class AppletIo : IAppletIO
    {
        /*
         * The components are :
         * Header = 1
         * Direcrtory = 2
         * Applet = 3
         * Import = 4
         * ConstantPool = 5
         * Class = 6
         * Method = 7
         * StaticField = 8
         * ReferenceLocation = 9
         * Export = 10
         * Descriptor = 11
         *
         * And the order to write is:  1, 2, 4, 3, 6, 7, 8, 10, 5, 9, 11 (optional)
         *
         * See also chapter 6.1 and 6.2 in the spec
        */

        private List<int> writingOrder = new List<int>() { 1, 2, 4, 3, 6, 7, 8, 10, 5, 9, 11 };

        internal AppletIo()
        {
        }

        public IEnumerable<AppletComponent> Read(string file)
        {
            IEnumerable<AppletComponent> component = null;

            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (String.IsNullOrEmpty(file))
            {
                throw new ArgumentException("please provide a filename", "file");
            }

            bool exists = File.Exists(file);
            if (!exists)
            {
                throw new FileNotFoundException("applet cannot be found!");
            }

            var extension = Path.GetExtension(file);
            extension = extension.ToLower();

            var isIjc = extension.Equals(".ijc");
            var isCap = extension.Equals(".cap");
            var isTxt = extension.Equals(".txt");

            if (isIjc || isCap || isTxt)
            {
                if (isIjc)
                {
                    component = ReadIjcFile(file);
                }

                if (isCap)
                {
                    component = ReadCapFile(file);
                }
                if (isTxt)
                {
                    component = ReadHexFile(file);
                }

                if (component == null)
                {
                    throw new FeatureException("Could not read applet. Unknown type");
                }
            }
            else
            {
                throw new ArgumentException("Only IJC and CAP and TXT(Hexstrings) files are supported");
            }

            return component;
        }

        public void WriteAsByte(string file, IEnumerable<AppletComponent> components)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (components == null)
            {
                throw new ArgumentNullException("components");
            }

            if (components.Any())
            {
                try
                {
                    using (var fileStream = File.OpenWrite(file))
                    {
                        // Write the components in the specified writing order
                        foreach (var curOrderId in writingOrder)
                        {
                            var component = components.FirstOrDefault(c => c.Tag == curOrderId);

                            // Some components are optional
                            if (component != null)
                            {
                                WriteIjcComponent(component, fileStream);
                            }
                            
                        }
                    }
                }
                catch (IOException ioEx)
                {
                    throw new FeatureException("Could not write applet due to an I/O error", ioEx);
                }
                catch(Exception ex)
                {
                    throw new FeatureException("Could not write applet", ex);
                }
            }
        }

        public void WriteAsHex(string file, IEnumerable<AppletComponent> components)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (components == null)
            {
                throw new ArgumentNullException("components");
            }

            if (components.Any())
            {
                try
                {
                    using (var streamWriter = File.CreateText(file))
                    {
                        // Write the components in the specified writing order
                        foreach (var curOrderId in writingOrder)
                        {
                            var component = components.FirstOrDefault(c => c.Tag == curOrderId);

                            // Some components are optional
                            if (component != null)
                            {
                                WriteHexComponent(component, streamWriter);
                            }

                        }
                    }
                }
                catch (IOException ioEx)
                {
                    throw new FeatureException("Could not write applet due to an I/O error", ioEx);
                }
                catch (Exception ex)
                {
                    throw new FeatureException("Could not write applet", ex);
                }
            }
        }

        private IEnumerable<AppletComponent> ReadHexFile(string file)
        {
            var components = new List<AppletComponent>();
            //TODO:  Rename this service. It is a hex string formatter / parser and not TLV related
            HexFormatter formatter = new HexFormatter();
            string hexString;

            // Read the hex string
            using (var textStream = File.OpenText(file))
            {
                hexString = textStream.ReadToEnd();
                hexString.Trim();
            }

            var response = formatter.Validate(hexString);
            if (response.IsValid)
            {
                var bytes = formatter.Convert(hexString);
                components.AddRange(ReadFromBytes(bytes));
            }
            else
            {
                throw new FeatureException(response.ValidationError);
            }

            return components;
        }

        private IEnumerable<AppletComponent> ReadIjcFile(string file)
        {
            /*
             *  component {
                    u1 tag
                    u2 size
                    u1 info[]
                }
             */

            var components = new List<AppletComponent>();
            using (var fileStream = File.Open(file,FileMode.Open))
            {
                // There won't be any applet exceeding the Int32 boundaries...
                var length = Convert.ToInt32(fileStream.Length);
                var bytes = new byte[length];
                fileStream.Read(bytes, 0, length);

                // Read the components now
                components.AddRange(ReadFromBytes(bytes));
            }

            return components;
        }

        private IEnumerable<AppletComponent> ReadFromBytes(IEnumerable<byte> bytes)
        {
            var workingBuffer = bytes;
            List<AppletComponent> components = new List<AppletComponent>();

            while(workingBuffer.Any())
            {
                // Read Tag and Size
                var tag = workingBuffer.First();
                if (!writingOrder.Contains(tag))
                {
                    throw new FeatureException(String.Format("Invalid component detected with tag = {0}", tag));
                }

                var sizeBytes = workingBuffer.Skip(1).Take(2).ToArray();
                sizeBytes = sizeBytes.Reverse().ToArray();
                var size = BitConverter.ToUInt16(sizeBytes);

                workingBuffer = workingBuffer.Skip(3);
                // Read the Info Bytes
                var infoBytes = workingBuffer.Take(size);
                var component = new AppletComponent(tag, size, infoBytes);
                components.Add(component);

                workingBuffer = workingBuffer.Skip(size);
            }

            return components;
        }

        private IEnumerable<AppletComponent> ReadCapFile(string file)
        {
            using (var fileStream = File.Open(file, FileMode.Open))
            {
                var content = new List<AppletComponent>();
                using (var capContent = new ZipArchive(fileStream, ZipArchiveMode.Read))
                {
                    foreach (var currentEntry in capContent.Entries)
                    {
                        // Ignore the meta data and focus only on the .cap files.
                        if (currentEntry.FullName.EndsWith(".cap"))
                        {
                            var component = CreateComponent(currentEntry);
                            content.Add(component);
                        }
                        //else
                        //{
                        //    //TODO: What to do?  Can this be deleted?
                        //}
                    }
                }

                return content;
            }
        }

        /// <summary>
        /// Converts the stream to Applet Component 
        /// </summary>
        private AppletComponent CreateComponent(ZipArchiveEntry entry)
        {
            var component = new AppletComponent();
            using (var zipEntry = entry.Open())
            {
                component.Read(zipEntry);
            }

            return component;
        }

        private void WriteHexComponent(AppletComponent component, StreamWriter writer)
        {
            var buffer = new StringBuilder();
            buffer.AppendFormat("{0:X2}", component.Tag);

            byte[] sizeArray = BitConverter.GetBytes(component.Size);
            buffer.AppendFormat("{0:X2}{1:X2}", sizeArray[1], sizeArray[0]);

            foreach (var curByte in component.Data)
            {
                buffer.AppendFormat("{0:X2}", curByte);
            }

            writer.Write(buffer.ToString());
        }

        private void WriteIjcComponent(AppletComponent component, FileStream stream)
        {
            var tagByte = Convert.ToByte(component.Tag);
            byte[] sizeArray = BitConverter.GetBytes(component.Size);

            stream.WriteByte(tagByte);
            stream.WriteByte(sizeArray[1]);
            stream.WriteByte(sizeArray[0]);
            stream.Write(component.Data.ToArray(), 0, component.Data.Count());
        }
    }
}

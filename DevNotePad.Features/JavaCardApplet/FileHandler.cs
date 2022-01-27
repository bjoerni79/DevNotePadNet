using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.JavaCardApplet
{
    internal class FileHandler
    {
        internal FileHandler()
        {
        }

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

        private List<int> writeOrder = new List<int>() { 1, 2, 4, 3, 6, 7, 8, 10, 5, 9, 11 };

        /// <summary>
        /// Reads the applet from the file and extract the cap components as AppletComponent instances
        /// </summary>
        internal IEnumerable<AppletComponent> ReadApplet(string file)
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
                            //if (Parameter.Verbose)
                            //{
                            //    System.Console.WriteLine("Processing {0}", currentEntry);
                            //}

                            var component = CreateComponent(currentEntry);
                            content.Add(component);
                        }
                        else
                        {
                            // For debugging only..
                            //if (Parameter.Verbose)
                            //{
                            //    System.Console.WriteLine("Ignoring {0}", currentEntry);
                            //}

                        }
                    }
                }

                return content;
            }


        }

        /// <summary>
        /// Writes the list of components to a file
        /// </summary>
        internal void WriteAppletAsString(IEnumerable<AppletComponent> components, string file)
        {
            using (var output = File.CreateText(file))
            {
                // write the components based on the order in the list
                foreach (var curId in writeOrder)
                {
                    // Get the component and throw an error if not available and required
                    var component = components.FirstOrDefault(c => c.Tag == curId);
                    if (component != null)
                    {
                        //if (Parameter.Verbose)
                        //{
                        //    Console.WriteLine("Writing component with ID {0}...", component.Tag);
                        //}

                        var hexString = ConvertToHexString(component);
                        output.Write(hexString);
                    }
                    else
                    {
                        //if (Parameter.Verbose)
                        //{
                        //    Console.WriteLine("Component with ID {0} could not be found!", curId);
                        //}


                    }


                }
            }
        }


        /// <summary>
        /// Writes the list of components to a file
        /// </summary>
        internal void WriteAppletAsByte(IEnumerable<AppletComponent> components, string file)
        {
            using (var output = File.Open(file, FileMode.Create))
            {
                // write the components based on the order in the list
                foreach (var curId in writeOrder)
                {
                    // Get the component and throw an error if not available and required
                    var component = components.FirstOrDefault(c => c.Tag == curId);
                    if (component != null)
                    {
                        //if (Parameter.Verbose)
                        //{
                        //    Console.WriteLine("Writing component with ID {0}...", component.Tag);
                        //}
                        WriteComponent(component, output);
                    }
                    else
                    {
                        //if (Parameter.Verbose)
                        //{
                        //    Console.WriteLine("Component with ID {0} could not be found!", curId);
                        //}
                    }
                }
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


        private string ConvertToHexString(AppletComponent component)
        {
            var buffer = new StringBuilder();
            buffer.AppendFormat("{0:X2}", component.Tag);

            byte[] sizeArray = BitConverter.GetBytes(component.Size);
            buffer.AppendFormat("{0:X2}{1:X2}", sizeArray[1], sizeArray[0]);

            foreach (var curByte in component.Data)
            {
                buffer.AppendFormat("{0:X2}", curByte);
            }

            return buffer.ToString();
        }

        private void WriteComponent(AppletComponent component, FileStream stream)
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

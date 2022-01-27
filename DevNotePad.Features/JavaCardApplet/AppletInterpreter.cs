using DevNotePad.Features.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Features.JavaCardApplet
{
    internal class AppletInterpreter : IAppletInterpreter
    {
        /*
         *  1  = Header
         *  2  = Directory
         *  3  = Applet
         *  4  = Import
         *  5  = Constant Pool
         *  6  = Class
         *  7  = Method
         *  8  = Static Field
         *  9  = Reference Location
         *  10 = Export
         *  11 = Descriptor
         */

        private const int Header = 1;
        private const int Directory = 2;
        private const int Applet = 3;
        private const int Import = 4;
        private const int ConstantPool = 5;
        private const int Class = 6;
        private const int Method = 7;
        private const int StaticField = 8;
        private const int ReferenceLocation = 9;
        private const int Export = 10;
        private const int Descriptor = 11;

        private readonly byte[] AppletMagicBytes = new byte[] { 0xDE, 0xCA, 0xFF, 0xED };

        private Dictionary<int, string> componentNameDict;

        internal AppletInterpreter()
        {
            componentNameDict = new Dictionary<int, string>();
            componentNameDict.Add(Header, "Header");
            componentNameDict.Add(Directory, "Directory");
            componentNameDict.Add(Applet, "Applet");
            componentNameDict.Add(Import, "Import");
            componentNameDict.Add(ConstantPool, "Constant Pool");
            componentNameDict.Add(Class, "Class");
            componentNameDict.Add(Method, "Method");
            componentNameDict.Add(StaticField, "Static Field");
            componentNameDict.Add(ReferenceLocation, "Reference Location");
            componentNameDict.Add(Export, "Export");
            componentNameDict.Add(Descriptor, "Descriptor");

        }

        public AppletComponentReport Decode(AppletComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException();
            }

            // Get the component type
            if (!componentNameDict.ContainsKey(component.Tag))
            {
                throw new FeatureException(String.Format("Unknown Applet Component detected! Tag={0}",component.Tag));
            }

            // Start the interpretation now
            var report = new AppletComponentReport();
            report.ComponentId = component.Tag;
            report.ComponentTitle = componentNameDict[component.Tag];
            report.RawValue = component.Data;

            string interpretedValue;
            switch (component.Tag)
            {
                case Header:
                    interpretedValue = DecodeHeaderComponent(component.Data);
                    break;
                //case Directory:
                //    interpretedValue = DecodeDirectoryComponent(component.Data);
                //    break;
                case Applet:
                    interpretedValue = DecodeAppletComponent(component.Data);
                    break;
                case Import:
                    interpretedValue = DecodeImportComponent(component.Data);
                    break;
                default:
                    interpretedValue = "No Interpretation. Please refer to the JCVM Spec";
                    break;
            }

            // Build the entire report including raw bytes now
            var hexConverter = new HexConverter();
            var sb = new StringBuilder();
            sb.AppendLine(interpretedValue);
            sb.AppendLine();
            sb.AppendLine("Raw Data:");
            sb.AppendLine(hexConverter.ConvertToHexString(component.Data));
            report.InterpretedValue = sb.ToString();

            return report;
        }

        private string DecodeHeaderComponent(IEnumerable<byte> data)
        {
            string interpretation;
            /*
             * See also Chapter 6.3 in JCVM Spec
             * 
             * header_component {
                u1 tag
                u2 size
                u4 magic
                u1 minor_version
                u1 major_version
                u1 flags
                package_info this_package
                }
             */

            var workingBuffer = data;

            // Continue only if the Magic Bytes are detected
            var magicBytes = workingBuffer.Take(4);
            if (magicBytes.SequenceEqual(AppletMagicBytes))
            {
                var sb = new StringBuilder();
                sb.Append("Magic Bytes DECAFFED detected\n");
                workingBuffer = workingBuffer.Skip(4);

                // Read the minor and major version
                var minor = workingBuffer.First();
                var major = workingBuffer.Skip(1).First();
                sb.AppendFormat("Requested JVM Release : {0}.{1}\n", major, minor);
                workingBuffer = workingBuffer.Skip(2);

                // Read the flags
                var flagByte = workingBuffer.First();
                string flagMeaning;
                switch (flagByte)
                {
                    case 0x01:
                        flagMeaning = "INT (01)";
                        break;
                    case 0x02:
                        flagMeaning = "EXPORT (02)";
                        break;
                    case 0x04:
                        flagMeaning = "Applet (04)";
                        break;
                    default:
                        flagMeaning = String.Format("Unknown ({0}", flagByte);
                        break;
                }

                sb.AppendFormat("Flag : {0}\n", flagMeaning);
                workingBuffer = workingBuffer.Skip(1);

                // Read the package info
                GetPackageInfo(workingBuffer, sb);

                // Interpretation Done!
                interpretation = sb.ToString();
            }
            else
            {
                interpretation = "Error: Magic Bytes not detected!";
            }

            return interpretation;
        }

        //private string DecodeDirectoryComponent(IEnumerable<byte> data)
        //{
        //    /*
        //     * See also JVM Spec 6.4
        //     * 
        //     * irectory_component {
        //        u1 tag
        //        u2 size
        //        u2 component_sizes[11]
        //        static_field_size_info static_field_size
        //        u1 import_count
        //        u1 applet_count
        //        u1 custom_count
        //        custom_component_info custom_components[custom_count]
        //        }
        //     * 
        //     */

        //    var componentSize = new List<int>();
        //    var workingBuffer = data;

        //    // Read the array of [0..10] 11 length codings. Iterate to pos 22 (2*11)
        //    //for (int pos = 0; pos <= 21; pos += 2)
        //    //{

        //    //}

        //    // format..
        //    workingBuffer = workingBuffer.Skip(22);

        //    // Static Field Size Info

        //    // Import Count

        //    // Applet Count

        //    // Custom Count

        //    // Custom Info

        //    return "Todo";
        //}

        private string DecodeAppletComponent(IEnumerable<byte> data)
        {
            var sb = new StringBuilder();
            var hexConverter = new HexConverter();
            var workingBuffer = data;
            var appletCount = workingBuffer.First();
            if (appletCount > 0)
            {
                workingBuffer = workingBuffer.Skip(1);

                // Read the AIDs of all detected applets
                for (int curApplet=0; curApplet < appletCount; curApplet++)
                {
                    var length = workingBuffer.First();
                    var aid = workingBuffer.Skip(1).Take(length).ToArray();
                    var installMethodOffset = workingBuffer.Skip(1).Skip(length).Take(2).ToArray();

                    // Install Method Offset (2) + Length (1) + length;
                    workingBuffer = workingBuffer.Skip(3 + length);
                    sb.AppendFormat("AID : {0} with Install Method Offset {1}\n", hexConverter.ConvertToHexString(aid), hexConverter.ConvertToHexString(installMethodOffset));
                }

                return sb.ToString();
            }
            else
            {
                return "None";
            }
        }

        private string DecodeImportComponent(IEnumerable<byte> data)
        {
            var sb = new StringBuilder();
            sb.Append("Imports:\n");

            var workingBuffer = data;

            // Read the number of imports
            var importCount = workingBuffer.First();
            workingBuffer = workingBuffer.Skip(1);
            
            for (int curImport = 0; curImport <importCount; curImport++)
            {
                var bytesRead = GetPackageInfo(workingBuffer, sb);
                workingBuffer = workingBuffer.Skip(bytesRead);
            }

            return sb.ToString();
        }

        private int GetPackageInfo(IEnumerable<byte> packageBytes,StringBuilder builder)
        {
            var hexConverter = new HexConverter();
            var minorVersion = packageBytes.First();
            var majorVersion = packageBytes.Skip(1).First();
            var aidLength = packageBytes.Skip(2).First();
            var aid = packageBytes.Skip(3).Take(aidLength);

            var packageInfo = String.Format("AID {0} with Version {1}.{2}\n", hexConverter.ConvertToHexString(aid), majorVersion, minorVersion);
            builder.Append(packageInfo);
            return 3 + aidLength;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DevNotePad.Shared
{
    public class Settings
    {

        public Settings()
        {
            EditorFontSize = 12;
            DefaultPath = null;
            LineWrap = false;
            ScratchPadEnabled = false;
            EditorFontSize = 12;
            IgnoreChanged = true;
            IgnoreReload = true;
            IgnoreOverwriteChanges = false;
            DefaultPath = null;
        }

        #region Properties

        /// <summary>
        /// Internal release scheme for future updates
        /// </summary>
        public string Version => "1.0";

        public int EditorFontSize { get; set; }

        /// <summary>
        /// If enabled, ignores the changes of any pending files on close or new/load
        /// </summary>
        public bool IgnoreChanged { get; set; }

        /// <summary>
        /// If enabled, ignores the changes on the local copy when performing the reload
        /// </summary>
        public bool IgnoreReload { get; set; }

        /// <summary>
        /// If enabled, ignores the changes on the hard disk created after the file has been opened in the editor (time stamp check)
        /// </summary>
        public bool IgnoreOverwriteChanges { get; set; }

        public string? DefaultPath { get; set; }

        public bool LineWrap { get; set; }

        public bool ScratchPadEnabled { get; set; }

        #endregion

        #region Static 

        private static string configFile = "devnotepad_config.xml";
        private static string ConfigNode = "Config";
        private static string VersionNode = "Version";
        private static string LineWrapNode = "LineWrap";
        private static string ScratchPadEnabledNode = "ScratchPadEnabled";
        private static string EditorFontSizeNode = "EditorFontSize";
        private static string IgnoreChangesNode = "IgnoreChanges";
        private static string IgnoreReloadNode = "IgnoreReload";
        private static string IgnoreOverwriteChangesNode = "IgnoreOverwriteChanges";
        private static string DefaultPathNode = "DefaultPath";

        public static Settings GetDefault()
        {
            Settings settings;

            try
            {
                // Load the defaults if no file can be found
                var fileExists = IsConfigFileAvailable();
                if (fileExists)
                {
                    // Read from Home Dir
                    settings = Read();
                }
                else
                {
                    // Set defaults
                    settings = new Settings();

                    // Write to home Dir
                    Write(settings);
                }
            }
            catch
            {
                // TODO: A excepion cannot be thrown and work with the default. This can can be improved
                settings = new Settings();
            }

            return settings;
        }

        /// <summary>
        /// Returns the file path to the config file
        /// </summary>
        /// <returns>true if available</returns>
        private static string GetFilePath()
        {
            var homeFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(homeFolderPath, configFile);
            return filePath;
        }

        /// <summary>
        /// Returns true if the config file is available
        /// </summary>
        /// <returns>true or false</returns>
        private static bool IsConfigFileAvailable()
        {
            var filePath = GetFilePath();
            return File.Exists(filePath);
        }

        public static void Write(Settings settings)
        {
            var filePath = GetFilePath();

            var xmlSettings = new XmlWriterSettings() {  Indent=true};
            using (var xmlWriter = XmlWriter.Create(filePath,xmlSettings))
            {
                xmlWriter.WriteStartDocument(true);

                // Start Group Node
                xmlWriter.WriteStartElement(ConfigNode);

                // Node Version
                xmlWriter.WriteElementString(VersionNode, settings.Version);

                // Node Line Wrap
                xmlWriter.WriteElementString(LineWrapNode, settings.LineWrap.ToString());

                // Node ScratchPadEnabled
                xmlWriter.WriteElementString(ScratchPadEnabledNode, settings.ScratchPadEnabled.ToString());

                // Editor Font Size
                xmlWriter.WriteElementString(EditorFontSizeNode, settings.EditorFontSize.ToString());

                // Ignore Changed
                xmlWriter.WriteElementString(IgnoreChangesNode, settings.IgnoreChanged.ToString());

                // Ignore Reload
                xmlWriter.WriteElementString(IgnoreReloadNode, settings.IgnoreReload.ToString());

                // Ignore Overwrite Changes
                xmlWriter.WriteElementString(IgnoreOverwriteChangesNode, settings.IgnoreOverwriteChanges.ToString());

                // Default Path
                if (settings.DefaultPath == null)
                {
                    xmlWriter.WriteElementString(DefaultPathNode, String.Empty);
                }
                else
                {
                    xmlWriter.WriteElementString(DefaultPathNode, settings.DefaultPath);
                }
                

                // End Group Node

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }

        public static Settings Read()
        {
            /*
             * ﻿<?xml version="1.0" encoding="utf-8" standalone="yes"?>
                <Config>
                  <Version>1.0</Version>
                  <LineWrap>False</LineWrap>
                  <ScratchPadEnabled>False</ScratchPadEnabled>
                  <EditorFontSize>12</EditorFontSize>
                  <IgnoreChanges>True</IgnoreChanges>
                  <IgnoreReload>True</IgnoreReload>
                  <IgnoreOverwriteChanges>False</IgnoreOverwriteChanges>
                  <DefaultPath />
                </Config>
             * 
             */

            var settings = new Settings();

            var filePath = GetFilePath();
            var xmlReaderSettings = new XmlReaderSettings();
            using (var xmlReader = XmlReader.Create(filePath,xmlReaderSettings))
            {
                string currentNode = String.Empty;
                string currentValue = String.Empty;

                while (xmlReader.Read())
                {
                    // Read the node and the value
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        currentNode = xmlReader.Name;
                    }

                    if (xmlReader.NodeType == XmlNodeType.Text)
                    {
                        currentValue = xmlReader.Value;
                    }

                    // Update the setting object
                    bool isConfigItem = ((currentNode != ConfigNode) && (currentNode != VersionNode));
                    if (isConfigItem)
                    {
                        // Apply LineWrap
                        if (currentNode == LineWrapNode)
                        {
                            settings.LineWrap = ReadBoolean(currentValue);
                        }

                        // ScratchPad Enabled
                        if (currentNode == ScratchPadEnabledNode)
                        {
                            settings.ScratchPadEnabled = ReadBoolean(currentValue);
                        }

                        // Ignore Changes
                        if (currentNode == IgnoreChangesNode)
                        {
                            settings.IgnoreChanged = ReadBoolean(currentValue);
                        }

                        // Ignore Reload
                        if (currentNode == IgnoreReloadNode)
                        {
                            settings.IgnoreReload = ReadBoolean(currentValue);
                        }

                        // Ignore Overwrite Changes
                        if (currentNode == IgnoreOverwriteChangesNode)
                        {
                            settings.IgnoreOverwriteChanges = ReadBoolean(currentValue);
                        }

                        // Default Path
                        if (currentNode == DefaultPathNode)
                        {
                            if (string.IsNullOrEmpty(currentValue))
                            {
                                settings.DefaultPath = null;
                            }
                            else
                            {
                                settings.DefaultPath = currentValue;
                            }
                        }

                        //TODO:  FontSize. At the moment it is 12 b default (set in the settings constructor)
                    }
                }
            }

            return settings;
        }

        private static bool ReadBoolean(string value)
        {
            bool booleanValue;
            var isSuccessful = Boolean.TryParse(value, out booleanValue);
            if (isSuccessful)
            {
                return booleanValue;
            }

            return false;
        }

        #endregion
    }
}

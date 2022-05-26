using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DevNotePad.Shared
{
    /// <summary>
    /// Helper class for dealing with FlowDocument classes
    /// </summary>
    public class FlowDocumentHelper
    {
        /// <summary>
        /// Creates a new instance of FlowDocumentHelper
        /// </summary>
        public FlowDocumentHelper()
        {

        }

        /// <summary>
        /// Creates a new and empty document
        /// </summary>
        /// <returns>A new FlowDocument</returns>
        public FlowDocument CreateEmpty()
        {
            var emptyParagraph = new Paragraph();
            var section = new Section(emptyParagraph);

            // Return the empty document
            return new FlowDocument(section);
        }

        /// <summary>
        /// Creates a flow document with the content of a string
        /// </summary>
        /// <param name="content">the content of the flow document</param>
        /// <returns>A new FlowDocument</returns>
        public FlowDocument Load(string content)
        {
            var rows = SplitContent(content);

            var section = new Section();
            foreach (var currentRow in rows)
            {
                var run = new Run(currentRow);
                var paragraph = new Paragraph(run);

                section.Blocks.Add(paragraph);
            }

            // Return the empty document
            return new FlowDocument(section);
        }

        /// <summary>
        /// Extracts the content from a flow document
        /// </summary>
        /// <param name="flowDocument">the flow document</param>
        /// <returns>the content as string</returns>
        public string Extract(FlowDocument flowDocument)
        {
            var contentBuilder = new StringBuilder();

            //  See MSDN page about FlowDocument : https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.flowdocument?f1url=%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(System.Windows.Documents.FlowDocument)%3Bk(DevLang-csharp)%26rd%3Dtrue&view=windowsdesktop-6.0
            //
            //  This methods extracts the Paragraph and Section. The other options will be ignored.
            //
            foreach (var block in flowDocument.Blocks)
            {
                var paragraph = block as Paragraph;
                var section = block as Section;

                if (paragraph != null)
                {
                    HandleParagraph(paragraph, contentBuilder);
                }

                if (section != null)
                {
                    HandleSection(section, contentBuilder);
                }
            }

            return contentBuilder.ToString();
        }

        private void HandleSection(Section section, StringBuilder stringBuilder)
        {
            // Iterate over each block in the section
            foreach (var block in section.Blocks)
            {
                var sectionBlock = block as Section;
                if (sectionBlock != null)
                {
                    HandleSection(sectionBlock, stringBuilder);
                }

                var paragraphBlock = block as Paragraph;
                if (paragraphBlock != null)
                {
                    HandleParagraph(paragraphBlock, stringBuilder);
                }
            }
        }

        private void HandleParagraph(Paragraph paragraph, StringBuilder stringBuilder)
        {
            // Read all the Inlines and add the string to the StringBuilder instance
            // MSDN page: https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.paragraph?f1url=%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(System.Windows.Documents.Paragraph)%3Bk(DevLang-csharp)%26rd%3Dtrue&view=windowsdesktop-6.0

            foreach (var inline in paragraph.Inlines)
            {
                // Bold
                var boldInline = inline as Bold;

                // Italic
                var italicInline = inline as Italic;

                // LineBreak
                var lineBreakInline = inline as LineBreak;
                if (lineBreakInline != null)
                {
                    stringBuilder.Append(Environment.NewLine);
                }

                // Run
                var runInline = inline as Run;
                if (runInline != null)
                {
                    var content = runInline.Text;
                    stringBuilder.Append(content);
                }

                // Span
                var spanInline = inline as Span;

                // Underline
                var underlineInLine = inline as Underline;


            }

            // Add a line break after each paragraph
            stringBuilder.Append(Environment.NewLine);
        }

        private IEnumerable<string> SplitContent(string content)
        {
            var splitToRows = content.Split(Environment.NewLine);
            return splitToRows;
        }


    }
}

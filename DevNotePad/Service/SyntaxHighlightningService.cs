using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace DevNotePad.Service
{
    public class SyntaxHighlightningService : ISyntaxHighlightningService
    {
        private bool isActive;

        public SyntaxHighlightningService()
        {
            // Always on for now. Helps debugging.
            isActive = true;
        }

        public void Refresh()
        {
            if (isActive)
            {
                //TODO: Iterate over all paragraphs...

            }
        }

        public void Refresh(Paragraph paragraph)
        {
            if (isActive && paragraph != null)
            {
                var rules = new List<SyntaxRule>()
                {
                    new SyntaxRule("public",Brushes.Blue),
                    new SyntaxRule("class", Brushes.Black)
                };

                // Parse them
                var text = ReadText(paragraph);

                SyntaxRule? currentRule = null;
                var formattedInlineList = new List<Inline>();

                int positionInKeyword = 0;
                bool doProcess = true;
                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                {
                    doProcess = false;
                }
                
                while (doProcess)
                {
                    var curChar = text.First();

                    // Rule checking
                    if (currentRule == null)
                    {
                        currentRule = FindRule(rules, curChar);
                        positionInKeyword = 0;
                    }

                    bool match = false;
                    if (currentRule != null)
                    {
                        match = currentRule.Test(curChar);

                        if (!currentRule.Hit)
                        {
                            // Merge somehow...
                            //
                            // Example: abcde
                            // 
                            // aa
                            // ab

                            currentRule.Reset();
                            currentRule = null;
                        }

                        positionInKeyword++;
                    }

                    if (match)
                    {
                        
                    }

                    // Go to the next character
                    // TODO: .NET 6 might offer a more effective ways of doing this. It works for now..
                    text = text.Remove(0, 1);
                    if (String.IsNullOrEmpty(text))
                    {
                        doProcess = false;
                    }
                }
            }
        }

        private SyntaxRule? FindRule (List<SyntaxRule> rules, char curChar)
        {
            SyntaxRule? ruleResult = null;

            // There is currently no rule in process.
            foreach (var rule in rules)
            {
                rule.Test(curChar);
                if (rule.Hit)
                {
                    ruleResult = rule;
                    break;
                }
            }

            return ruleResult;
        }

        private string ReadText(Paragraph paragraph)
        {
            // Add all text to one buffer. 
            var inlines = paragraph.Inlines;
            var contentBuilder = new StringBuilder();

            foreach (var inline in inlines)
            {
                var run = inline as Run;
                if (run != null)
                {
                    contentBuilder.Append(run.Text);
                }

                //TODO: Spans?

            }

            return contentBuilder.ToString();
        }

        public void Start()
        {
            isActive = true;
        }

        public void Stop()
        {
            isActive = false;
        }
    }
}

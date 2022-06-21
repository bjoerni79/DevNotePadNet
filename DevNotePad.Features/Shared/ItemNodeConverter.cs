using System.Text;

namespace DevNotePad.Features.Shared
{
    internal class ItemNodeConverter
    {
        internal ItemNodeConverter()
        {

        }

        internal string ToTreeAsString(ItemNode node)
        {
            var reportBuilder = new StringBuilder();
            Read(node, reportBuilder, 0);

            return reportBuilder.ToString();
        }

        private void Read(ItemNode node, StringBuilder reportBuilder, int level)
        {
            // Add a tab character for each level
            string levelIntend = String.Empty;
            for (int curLevel = 0; curLevel < level; curLevel++)
            {
                levelIntend = String.Format("{0}\t", levelIntend);
            }

            reportBuilder.AppendFormat("{0} {1} {2}\n", levelIntend, node.Name, node.Description);

            if (node.Childs.Any())
            {
                foreach (var child in node.Childs)
                {
                    Read(child, reportBuilder, level + 1);
                }
            }
        }
    }
}

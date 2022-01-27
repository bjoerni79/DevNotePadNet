using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Shared
{
    public class AppletComponentViewItem
    {
        public AppletComponentViewItem()
        {
            Title = "Unknown";
            Content = "None";
        }

        public string Title { get; set; }
        public string Id { get; set; }
        public string Content { get; set; }
    }
}

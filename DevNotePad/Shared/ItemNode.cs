using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.Shared
{
    public class ItemNode
    {
        public ItemNode()
        {
            Name = "Unknown";
            Description = String.Empty;
            Childs = new ObservableCollection<ItemNode>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public ObservableCollection<ItemNode> Childs { get; set; }

    }
}

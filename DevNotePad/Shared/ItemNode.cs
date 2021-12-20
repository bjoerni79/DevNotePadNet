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
            Value = "None";
            Childs = new ObservableCollection<ItemNode>();
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public ObservableCollection<ItemNode> Childs { get; set; }

    }
}

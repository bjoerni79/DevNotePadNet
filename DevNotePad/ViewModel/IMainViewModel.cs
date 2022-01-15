using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public interface IMainViewModel
    {
        void Init(IMainViewUi ui, ITextComponent text);

        void NotifyContentChanged(int added, int offset, int removed);
        void ApplySettings();

        bool IsChanged();
    }
}

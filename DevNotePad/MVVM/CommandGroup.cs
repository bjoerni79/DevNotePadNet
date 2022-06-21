using Generic.MVVM;
using System.Collections.Generic;

namespace DevNotePad.MVVM
{
    internal class CommandGroup
    {
        internal IEnumerable<IRefreshCommand> Commands { get; private set; }

        internal CommandGroup(IEnumerable<IRefreshCommand> commands)
        {
            Commands = commands;
        }

        internal void Refresh()
        {
            App.Current.Dispatcher.BeginInvoke(InternalRefresh);
        }

        private void InternalRefresh()
        {
            foreach (var command in Commands)
            {
                if (command != null)
                {
                    command.Refresh();
                }

            }
        }
    }
}

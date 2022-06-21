using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace DevNotePad.MVVM
{
    internal class RelayCommandGroup
    {
        internal IEnumerable<RelayCommand> Commands { get; private set; }

        internal RelayCommandGroup(IEnumerable<RelayCommand> commands)
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
                    command.NotifyCanExecuteChanged();
                }

            }
        }
    }
}

using System.IO;
using System.Windows;

namespace DevNotePad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        public Bootstrap BootStrap => Resources[Bootstrap.BootstrapId] as Bootstrap;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // If there is a file in the first argument index then open it as text
            var args = e.Args;
            if (args != null && args.Length > 0)
            {
                var fileName = args[0];

                var isValid = File.Exists(fileName);
                if (isValid)
                {
                    StartUpCondition.FileName = fileName;
                }
            }

            // Init the bootstrapper
            var bootstrap = Resources[Bootstrap.BootstrapId] as Bootstrap;
            if (bootstrap != null)
            {
                bootstrap.Init();
            }

        }

        //TODO: https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }
}

using System.IO;
using System.Windows;

namespace DevNotePad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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

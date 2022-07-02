using CommunityToolkit.Mvvm.Messaging;
using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared;
using DevNotePad.ViewModel;
using Generic.MVVM.Event;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DevNotePad
{
    public class Bootstrap
    {
        // See ViewModelInstances and Events for other
        public const string BootstrapId = "bootstrap";
        public const string EventControllerId = "eventcontrollerid";
        public const string DialogServiceId = "dialogserviceid";
        public const string ToolDialogServiceId = "tooldialogserviceid";
        public const string IoServiceId = "ioserviceid";
        public const string SettingsId = "settingsid";

        public Bootstrap()
        {
            FacadeFactory.InitFactory();

            Main = new MainViewModel();
        }

        public void Init()
        {
            InitComponents();
            LoadSettings();

            BuildServices();
        }

        private void InitComponents()
        {
            IIoService ioService = new IoService();

            // Add it to the IoC container 
            var facade = FacadeFactory.Create();
            facade.AddUnique(ioService, IoServiceId);
        }

        private IServiceProvider BuildServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IIoService, IoService>();

            return services.BuildServiceProvider();
        }

        private void LoadSettings()
        {
            //TODO: Load the settings, if found..
            var settings = Settings.GetDefault();

            // Store it in the container
            var facade = FacadeFactory.Create();
            facade.AddUnique(settings, SettingsId);
        }

        public IServiceProvider Services { get; }

        public MainViewModel Main { get; set; }


    }
}

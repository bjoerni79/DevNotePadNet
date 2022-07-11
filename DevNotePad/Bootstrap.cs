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
        //public const string SettingsId = "settingsid";

        public Bootstrap()
        {
            Main = new MainViewModel();
        }

        public void Init()
        {
            Services = BuildServices();

        }

        private IServiceProvider BuildServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IIoService, IoService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IToolDialogService, ToolDialogService>();
            services.AddSingleton<ISettingsService, SettingsService>();

            //TODO: ViewModels

            return services.BuildServiceProvider();
        }

        public IServiceProvider Services { get; internal set; }

        public MainViewModel Main { get; set; }


    }
}

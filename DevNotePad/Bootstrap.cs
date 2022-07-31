using CommunityToolkit.Mvvm.Messaging;
using DevNotePad.MVVM;
using DevNotePad.Service;
using DevNotePad.Shared;
using DevNotePad.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DevNotePad
{
    public class Bootstrap
    {
        // See ViewModelInstances and Events for other
        public const string BootstrapId = "bootstrap";

        public Bootstrap()
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
            // MainView Model
            services.AddTransient<MainViewModel>();
            // Settings View Model
            services.AddTransient<SettingsDialogViewModel>();
            // Tool Dialog View Model
            services.AddTransient<Base64ToolViewModel>();
            services.AddTransient<FindDialogViewModel>();
            services.AddTransient<ReplaceDialogViewModel>();
            services.AddTransient<XPathQueryViewModel>();
            services.AddTransient<XsltTransformerViewModel>();
            services.AddTransient<XmlSchemaValidatorViewModel>();
            services.AddTransient<RegularExpressionViewModel>();
            services.AddTransient<GuidCreatorViewModel>();
            services.AddTransient<TreeViewModel>();

            // Other VM
            services.AddTransient<ConfimDialogViewModel>();
            services.AddTransient<OkDialogViewModel>();
            services.AddTransient<NotifierViewModel>();

            return services.BuildServiceProvider();
        }

        public IServiceProvider Services { get; internal set; }



    }
}

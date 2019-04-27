using System.IO;
using System.Reflection;
using System.Windows;
using App.Logic;
using App.Operations;
using App.ViewModels;
using Autofac;
using SettingsManager;
using SettingsManager.ModelProcessors;

namespace App
{
    public partial class AppContainer
    {
        private IContainer _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = new ContainerBuilder();

            // windows
            container.RegisterType<MainWindow>();
            container.RegisterType<NewMappingWindow>();

            // view models
            container.RegisterType<MainWindowViewModel>().SingleInstance();
            container.RegisterType<NewMappingWindowViewModel>();

            // operations
            container.RegisterType<MappingOperation>().SingleInstance();

            container.Register<AppSettings>(context =>
            {
                return new SettingsBuilder<AppSettings>()
                    .WithFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "appSettings.json"))
                    .WithProcessor(new JsonModelProcessor())
                    .Build();
            }).SingleInstance();
            container.RegisterType<HooksHandler>().SingleInstance();
            container.RegisterGeneric(typeof(Provider<>)).SingleInstance();

            _container = container.Build();

            _container.Resolve<MainWindow>().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _container.Resolve<HooksHandler>().Dispose();
            base.OnExit(e);
        }
    }
}

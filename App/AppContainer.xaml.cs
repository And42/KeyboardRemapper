using System.IO;
using System.Windows;
using System.Windows.Forms;
using App.Logic;
using App.Logic.Operations;
using App.Logic.Utils;
using App.ViewModels;
using App.Windows;
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
            container.RegisterType<MainWindow>().SingleInstance();
            container.RegisterType<NewMappingWindow>();

            // view models
            container.RegisterType<MainWindowViewModel>().SingleInstance();
            container.RegisterType<NewMappingWindowViewModel>();

            // operations
            container.RegisterType<MappingOperation>().SingleInstance();

            // utils
            container.RegisterType<AppUtils>().SingleInstance();
            container.RegisterType<ThemingUtils>().SingleInstance();

            // other
            container.Register(context => new SettingsBuilder<AppSettings>()
                .WithFile(Path.Combine(context.Resolve<AppUtils>().GetExecutableDir() ?? string.Empty, "appSettings.json"))
                .WithProcessor(new JsonModelProcessor())
                .Build()
            ).SingleInstance().As<AppSettings>();
            container.RegisterType<HooksHandler>().SingleInstance();
            container.RegisterGeneric(typeof(Provider<>)).SingleInstance();
            container.RegisterType<NotifyIcon>().SingleInstance();
            container.RegisterType<NotifyIconHolder>().SingleInstance();
            container.RegisterType<KeyMappingsHandler>().SingleInstance();

            _container = container.Build();

            Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _container.Resolve<KeyMappingsHandler>().Dispose();
            _container.Resolve<HooksHandler>().Dispose();
            _container.Resolve<NotifyIconHolder>().Dispose();

            base.OnExit(e);
        }

        private void Initialize()
        {
            // constructor invocation starts hooking
            _container.Resolve<KeyMappingsHandler>();

            // apply app theme
            _container.Resolve<ThemingUtils>().ApplyCurrent();

            // start minimized if needed
            var mainWindow = _container.Resolve<MainWindow>();

            if (_container.Resolve<AppSettings>().StartMinimized)
            {
                mainWindow.WindowState = WindowState.Minimized;
                mainWindow.ShowInTaskbar = false;
                _container.Resolve<NotifyIconHolder>().NotifyIcon.Visible = true;
            }

            mainWindow.Show();
        }
    }
}

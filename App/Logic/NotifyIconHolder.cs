using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using App.Annotations;
using App.Properties;

namespace App.Logic
{
    public class NotifyIconHolder : IDisposable
    {
        [NotNull] public NotifyIcon NotifyIcon { get; }

        [NotNull] private readonly Provider<MainWindow> _mainWindowProvider;

        public NotifyIconHolder(
            [NotNull] NotifyIcon notifyIcon,
            [NotNull] Provider<MainWindow> mainWindowProvider
        )
        {
            NotifyIcon = notifyIcon;
            _mainWindowProvider = mainWindowProvider;

            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            notifyIcon.Text = Resources.AppTitle;
            notifyIcon.MouseClick += NotifyIcon_OnMouseClick;
        }

        private void NotifyIcon_OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            _mainWindowProvider.Get().WindowState = WindowState.Normal;
            _mainWindowProvider.Get().Activate();
        }

        public void Dispose()
        {
            NotifyIcon.Dispose();
        }
    }
}

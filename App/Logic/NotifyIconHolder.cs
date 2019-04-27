using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using App.Annotations;
using App.Properties;
using App.Utils;

namespace App.Logic
{
    public class NotifyIconHolder : IDisposable
    {
        [NotNull] public NotifyIcon NotifyIcon { get; }

        [NotNull] private readonly Provider<MainWindow> _mainWindowProvider;

        public NotifyIconHolder(
            [NotNull] NotifyIcon notifyIcon,
            [NotNull] Provider<MainWindow> mainWindowProvider,
            [NotNull] AppUtils appUtils
        )
        {
            NotifyIcon = notifyIcon;
            _mainWindowProvider = mainWindowProvider;

            notifyIcon.Icon = Icon.ExtractAssociatedIcon(appUtils.GetExecutablePath());
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

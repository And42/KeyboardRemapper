using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using App.Interfaces.Logic;
using App.Interfaces.Logic.Utils;
using App.Properties;
using App.Windows;
using JetBrains.Annotations;

namespace App.Logic
{
    public class NotifyIconHolder : INotifyIconHolder
    {
        [NotNull] public NotifyIcon NotifyIcon { get; }

        [NotNull] private readonly IProvider<MainWindow> _mainWindowProvider;

        public NotifyIconHolder(
            [NotNull] NotifyIcon notifyIcon,
            [NotNull] IProvider<MainWindow> mainWindowProvider,
            [NotNull] IAppUtils appUtils
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
            _mainWindowProvider.Get().Show();
        }

        public void Dispose()
        {
            NotifyIcon.Dispose();
        }
    }
}

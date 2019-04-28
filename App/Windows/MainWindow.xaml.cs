using System;
using System.Windows;
using App.Interfaces.Logic;
using App.Interfaces.ViewModels;
using JetBrains.Annotations;

namespace App.Windows
{
    public partial class MainWindow
    {
        [NotNull] private readonly INotifyIconHolder _notifyIconHolder;

        public MainWindow(
            [NotNull] IMainWindowViewModel viewModel,
            [NotNull] INotifyIconHolder notifyIconHolder
        )
        {
            _notifyIconHolder = notifyIconHolder;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Window_OnStateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Minimized:
                    ShowInTaskbar = false;
                    _notifyIconHolder.NotifyIcon.Visible = true;
                    break;
                default:
                    ShowInTaskbar = true;
                    _notifyIconHolder.NotifyIcon.Visible = false;
                    break;
            }
        }
    }
}

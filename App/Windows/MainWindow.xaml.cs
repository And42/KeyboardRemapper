using System;
using System.Windows;
using App.Logic;
using App.ViewModels;
using JetBrains.Annotations;

namespace App.Windows
{
    public partial class MainWindow
    {
        [NotNull] private readonly NotifyIconHolder _notifyIconHolder;

        public MainWindow(
            [NotNull] MainWindowViewModel viewModel,
            [NotNull] NotifyIconHolder notifyIconHolder
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

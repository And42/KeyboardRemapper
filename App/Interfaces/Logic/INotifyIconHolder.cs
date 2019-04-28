using System;
using System.Windows.Forms;

namespace App.Interfaces.Logic
{
    public interface INotifyIconHolder : IDisposable
    {
        NotifyIcon NotifyIcon { get; }
    }
}
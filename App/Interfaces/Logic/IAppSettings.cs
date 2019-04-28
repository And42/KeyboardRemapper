using System.Collections.Generic;
using System.ComponentModel;
using App.Logic;

namespace App.Interfaces.Logic
{
    public interface IAppSettings : INotifyPropertyChanged
    {
        int SettingsVersion { get; set; }

        IReadOnlyDictionary<int, int> KeyMappings { get; set; }

        AppThemes AppTheme { get; set; }

        bool StartMinimized { get; set; }
    }
}
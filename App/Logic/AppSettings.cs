using System.Collections.Generic;
using SettingsManager;

namespace App.Logic
{
    public class AppSettings : SettingsModel
    {
        public virtual int SettingsVersion { get; set; } = 2;

        public virtual IReadOnlyDictionary<int, int> KeyMappings { get; set; }

        public virtual AppThemes AppTheme { get; set; } = AppThemes.Light;

        public virtual bool StartMinimized { get; set; } = true;
    }
}

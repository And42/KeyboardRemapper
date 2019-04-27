using System.Collections.Generic;
using SettingsManager;

namespace App.Logic
{
    public class AppSettings : SettingsModel
    {
        public virtual int SettingsVersion { get; set; } = 0;

        public virtual IReadOnlyDictionary<int, int> KeyMappings { get; set; }
    }
}

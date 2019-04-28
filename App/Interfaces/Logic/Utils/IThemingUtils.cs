using System.Collections.Generic;
using System.ComponentModel;
using App.Logic;

namespace App.Interfaces.Logic.Utils
{
    public interface IThemingUtils : INotifyPropertyChanged
    {
        AppThemes CurrentTheme { get; set; }

        IReadOnlyList<AppThemes> AvailableThemes { get; }

        void ApplyCurrent();
    }
}
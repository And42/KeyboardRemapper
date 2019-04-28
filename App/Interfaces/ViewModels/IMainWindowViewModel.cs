using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using App.Logic;
using App.ViewModels;
using MVVM_Tools.Code.Commands;

namespace App.Interfaces.ViewModels
{
    public interface IMainWindowViewModel : INotifyPropertyChanged
    {
        ObservableCollection<KeyToKeyViewModel> KeyMappings { get; }

        IReadOnlyList<AppThemes> AvailableThemes { get; }

        bool StartMinimized { get; set; }

        bool StartWithWindows { get; set; }

        AppThemes AppTheme { get; set; }

        KeyToKeyViewModel SelectedKey { get; set; }

        IActionCommand AddMappingCommand { get; }
        IActionCommand ClearMappingsCommand { get; }
        IActionCommand DeleteMappingCommand { get; }
        IActionCommand ExportMappingsCommand { get; }
        IActionCommand ImportMappingsCommand { get; }
    }
}
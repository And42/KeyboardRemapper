using System.Collections.Generic;
using System.ComponentModel;
using App.Logic;
using MVVM_Tools.Code.Commands;

namespace App.Interfaces.ViewModels
{
    public interface INewMappingWindowViewModel : INotifyPropertyChanged
    {
        IReadOnlyList<int> AvailableKeys { get; }

        int SourceKey { get; set; }

        int MappedKey { get; set; }

        RecordingStates RecordingState { get; }

        IActionCommand<int> SetSourceKeyCommand { get; }
        IActionCommand<int> SetMappedKeyCommand { get; }
        IActionCommand RecordSourceKeyCommand { get; }
        IActionCommand RecordMappedKeyCommand { get; }
        IActionCommand<int> RecordKeyCommand { get; }
        IActionCommand StopRecordingCommand { get; }
        IActionCommand ApplyCommand { get; }     
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using WindowsInput.Native;
using App.Interfaces.ViewModels;
using App.Logic;
using App.Logic.Operations;
using JetBrains.Annotations;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;

namespace App.ViewModels
{
    public class NewMappingWindowViewModel : BindableBase, INewMappingWindowViewModel
    {
        public IReadOnlyList<int> AvailableKeys { get; } = Array.ConvertAll((VirtualKeyCode[]) Enum.GetValues(typeof(VirtualKeyCode)), it => (int) it);

        public int SourceKey
        {
            get => _sourceKey;
            set => SetProperty(ref _sourceKey, value);
        }

        public int MappedKey
        {
            get => _mappedKey;
            set => SetProperty(ref _mappedKey, value);
        }

        public RecordingStates RecordingState
        {
            get => _recordingState;
            private set => SetProperty(ref _recordingState, value);
        }

        public IActionCommand<int> SetSourceKeyCommand { get; }
        public IActionCommand<int> SetMappedKeyCommand { get; }
        public IActionCommand RecordSourceKeyCommand { get; }
        public IActionCommand RecordMappedKeyCommand { get; }
        public IActionCommand<int> RecordKeyCommand { get; }
        public IActionCommand StopRecordingCommand { get; }
        public IActionCommand ApplyCommand { get; }

        private int _sourceKey;
        private int _mappedKey;
        private RecordingStates _recordingState = RecordingStates.Idle;

        [NotNull] private readonly MappingOperation _mappingOperation;

        public NewMappingWindowViewModel(
            [NotNull] MappingOperation mappingOperation
        )
        {
            _mappingOperation = mappingOperation;

            SetSourceKeyCommand = new ActionCommand<int>(keyCode => SourceKey = keyCode);
            SetMappedKeyCommand = new ActionCommand<int>(keyCode => MappedKey = keyCode);
            RecordSourceKeyCommand = new ActionCommand(() => RecordingState = RecordingStates.SourceKey);
            RecordMappedKeyCommand = new ActionCommand(() => RecordingState = RecordingStates.MappedKey);
            RecordKeyCommand = new ActionCommand<int>(RecordKey_Execute, _ => RecordingState != RecordingStates.Idle);
            StopRecordingCommand = new ActionCommand(() => RecordingState = RecordingStates.Idle, () => RecordingState != RecordingStates.Idle);
            ApplyCommand = new ActionCommand(Apply_Execute);

            PropertyChanged += OnPropertyChanged;
        }

        #region Commands

        private void RecordKey_Execute(int keyCode)
        {
            if (_recordingState == RecordingStates.SourceKey)
                SourceKey = keyCode;
            else
                MappedKey = keyCode;

            RecordingState = RecordingStates.Idle;
        }

        private void Apply_Execute()
        {
            _mappingOperation.SourceKey = SourceKey;
            _mappingOperation.MappedKey = MappedKey;
            _mappingOperation.Success = true;
        }

        #endregion

        #region Property observers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(RecordingState):
                    StopRecordingCommand.RaiseCanExecuteChanged();
                    break;
            }
        }

        #endregion
    }
}

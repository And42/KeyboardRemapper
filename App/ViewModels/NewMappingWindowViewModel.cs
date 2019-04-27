using App.Annotations;
using App.Operations;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;

namespace App.ViewModels
{
    public class NewMappingWindowViewModel : BindableBase
    {
        public enum RecordingStates
        {
            Idle, SourceKey, MappedKey
        }

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

            RecordSourceKeyCommand = new ActionCommand(() => RecordingState = RecordingStates.SourceKey);
            RecordMappedKeyCommand = new ActionCommand(() => RecordingState = RecordingStates.MappedKey);
            RecordKeyCommand = new ActionCommand<int>(RecordKey_Execute, _ => RecordingState != RecordingStates.Idle);
            StopRecordingCommand = new ActionCommand(() => RecordingState = RecordingStates.Idle);
            ApplyCommand = new ActionCommand(Apply_Execute);
        }

        private void RecordKey_Execute(int keyCode)
        {
            if (_recordingState == RecordingStates.SourceKey)
                SourceKey = keyCode;
            else
                MappedKey = keyCode;
        }

        private void Apply_Execute()
        {
            _mappingOperation.SourceKey = SourceKey;
            _mappingOperation.MappedKey = MappedKey;
            _mappingOperation.Success = true;
        }
    }
}

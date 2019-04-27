using System;
using System.Collections.ObjectModel;
using System.Linq;
using WindowsInput;
using WindowsInput.Native;
using App.Annotations;
using App.Logic;
using App.Operations;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;

namespace App.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<KeyToKeyViewModel> KeyMappings { get; } = new ObservableCollection<KeyToKeyViewModel>();

        public IActionCommand AddMappingCommand { get; }
        public IActionCommand ClearMappingsCommand { get; }

        [NotNull] private readonly HooksHandler _hooksHandler;
        [NotNull] private readonly AppSettings _appSettings;
        [NotNull] private readonly Provider<NewMappingWindow> _newMappingWindowProvider;
        [NotNull] private readonly MappingOperation _mappingOperation;

        private readonly InputSimulator _inputSimulator = new InputSimulator();

        public MainWindowViewModel(
            [NotNull] HooksHandler hooksHandler,
            [NotNull] AppSettings appSettings,
            [NotNull] Provider<NewMappingWindow> newMappingWindowProvider,
            [NotNull] MappingOperation mappingOperation
        )
        {
            _hooksHandler = hooksHandler;
            _appSettings = appSettings;
            _newMappingWindowProvider = newMappingWindowProvider;
            _mappingOperation = mappingOperation;

            if (appSettings.KeyMappings != null)
            {
                foreach (var mapping in appSettings.KeyMappings)
                {
                    hooksHandler.AddHook(mapping.Key, CreateMappingHandler(mapping.Value));
                    KeyMappings.Add(new KeyToKeyViewModel {SourceKey = mapping.Key, MappedKey = mapping.Value});
                }
            }

            AddMappingCommand = new ActionCommand(AddMapping_Execute);
            ClearMappingsCommand = new ActionCommand(ClearMappings_Execute);
        }

        private void AddMapping_Execute()
        {
            _mappingOperation.Reset();
            _newMappingWindowProvider.Get().ShowDialog();

            if (_mappingOperation.Success)
            {
                _hooksHandler.AddHook(_mappingOperation.SourceKey, CreateMappingHandler(_mappingOperation.MappedKey));
                KeyMappings.Add(new KeyToKeyViewModel {SourceKey = _mappingOperation.SourceKey, MappedKey = _mappingOperation.MappedKey});
                UpdateSettings();
            }
        }

        private void ClearMappings_Execute()
        {
            _hooksHandler.RemoveAllHooks();
            KeyMappings.Clear();
            UpdateSettings();
        }

        private void UpdateSettings()
        {
            _appSettings.KeyMappings = KeyMappings.ToDictionary(it => it.SourceKey, it => it.MappedKey);
        }

        private Action CreateMappingHandler(int keyCode)
        {
            return () => _inputSimulator.Keyboard.KeyPress((VirtualKeyCode)keyCode);
        }
    }
}

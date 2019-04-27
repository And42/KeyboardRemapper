using System.Collections.ObjectModel;
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

        [NotNull] private readonly KeyMappingsHandler _keyMappingsHandler;
        [NotNull] private readonly Provider<NewMappingWindow> _newMappingWindowProvider;
        [NotNull] private readonly MappingOperation _mappingOperation;

        public MainWindowViewModel(
            [NotNull] KeyMappingsHandler keyMappingsHandler,
            [NotNull] Provider<NewMappingWindow> newMappingWindowProvider,
            [NotNull] MappingOperation mappingOperation
        )
        {
            _keyMappingsHandler = keyMappingsHandler;
            _newMappingWindowProvider = newMappingWindowProvider;
            _mappingOperation = mappingOperation;

            foreach (var mapping in keyMappingsHandler.KeyMappings)
                KeyMappings.Add(new KeyToKeyViewModel {SourceKey = mapping.Key, MappedKey = mapping.Value});

            AddMappingCommand = new ActionCommand(AddMapping_Execute);
            ClearMappingsCommand = new ActionCommand(ClearMappings_Execute);
        }

        private void AddMapping_Execute()
        {
            _mappingOperation.Reset();
            _newMappingWindowProvider.Get().ShowDialog();

            if (_mappingOperation.Success)
            {
                _keyMappingsHandler.AddMapping(_mappingOperation.SourceKey, _mappingOperation.MappedKey);
                KeyMappings.Add(new KeyToKeyViewModel {SourceKey = _mappingOperation.SourceKey, MappedKey = _mappingOperation.MappedKey});
            }
        }

        private void ClearMappings_Execute()
        {
            _keyMappingsHandler.RemoveAllMappings();
            KeyMappings.Clear();
        }
    }
}

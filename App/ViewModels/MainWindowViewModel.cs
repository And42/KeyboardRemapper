using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using App.Interfaces.Logic;
using App.Interfaces.Logic.Utils;
using App.Interfaces.ViewModels;
using App.Logic;
using App.Logic.JsonModels;
using App.Logic.Operations;
using App.Properties;
using App.Windows;
using JetBrains.Annotations;
using Microsoft.Win32;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;
using Newtonsoft.Json;

namespace App.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        private const string StartupRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public ObservableCollection<KeyToKeyViewModel> KeyMappings { get; } = new ObservableCollection<KeyToKeyViewModel>();

        public IReadOnlyList<AppThemes> AvailableThemes => _themingUtils.AvailableThemes;

        public bool StartWithWindows
        {
            get => _startWithWindows;
            set => SetProperty(ref _startWithWindows, value);
        }

        public bool StartMinimized
        {
            get => _appSettings.StartMinimized;
            set => _appSettings.StartMinimized = value;
        }

        public AppThemes AppTheme
        {
            get => _themingUtils.CurrentTheme;
            set => _themingUtils.CurrentTheme = value;
        }

        [CanBeNull]
        public KeyToKeyViewModel SelectedKey
        {
            get => _selectedKey;
            set => SetProperty(ref _selectedKey, value);
        }

        public IActionCommand AddMappingCommand { get; }
        public IActionCommand DeleteMappingCommand { get; }
        public IActionCommand ClearMappingsCommand { get; }
        public IActionCommand ImportMappingsCommand { get; }
        public IActionCommand ExportMappingsCommand { get; }

        [NotNull] private readonly IKeyMappingsHandler _keyMappingsHandler;
        [NotNull] private readonly IProvider<NewMappingWindow> _newMappingWindowProvider;
        [NotNull] private readonly MappingOperation _mappingOperation;
        [NotNull] private readonly IAppUtils _appUtils;
        [NotNull] private readonly IThemingUtils _themingUtils;
        [NotNull] private readonly IAppSettings _appSettings;

        private bool _startWithWindows;
        private KeyToKeyViewModel _selectedKey;

        public MainWindowViewModel(
            [NotNull] IKeyMappingsHandler keyMappingsHandler,
            [NotNull] IProvider<NewMappingWindow> newMappingWindowProvider,
            [NotNull] MappingOperation mappingOperation,
            [NotNull] IAppUtils appUtils,
            [NotNull] IThemingUtils themingUtils,
            [NotNull] IAppSettings appSettings
        )
        {
            _keyMappingsHandler = keyMappingsHandler;
            _newMappingWindowProvider = newMappingWindowProvider;
            _mappingOperation = mappingOperation;
            _appUtils = appUtils;
            _themingUtils = themingUtils;
            _appSettings = appSettings;

            foreach (var mapping in keyMappingsHandler.KeyMappings)
                KeyMappings.Add(new KeyToKeyViewModel {SourceKey = mapping.Key, MappedKey = mapping.Value});

            AddMappingCommand = new ActionCommand(AddMapping_Execute);
            DeleteMappingCommand = new ActionCommand(DeleteMapping_Execute, () => SelectedKey != null);
            ClearMappingsCommand = new ActionCommand(ClearMappings_Execute);
            ImportMappingsCommand = new ActionCommand(ImportMappings_Execute);
            ExportMappingsCommand = new ActionCommand(ExportMappings_Execute);
            _startWithWindows = GetStartupValue();

            PropertyChanged += OnPropertyChanged;
            _themingUtils.PropertyChanged += ThemingUtils_OnPropertyChanged;
            _appSettings.PropertyChanged += AppSettings_OnPropertyChanged;
        }

        #region Commands

        private void DeleteMapping_Execute()
        {
            if (SelectedKey == null)
                return;

            _keyMappingsHandler.RemoveMapping(SelectedKey.SourceKey);
            KeyMappings.Remove(SelectedKey);
        }

        private void AddMapping_Execute()
        {
            // disable all mappings to not confuse anyone who will be setting up a new mapping
            _keyMappingsHandler.RemoveAllMappings();

            _mappingOperation.Reset();
            _newMappingWindowProvider.Get().ShowDialog();

            // restore all the disabled mappings
            foreach (var mapping in KeyMappings)
                _keyMappingsHandler.SetMapping(mapping.SourceKey, mapping.MappedKey);

            if (_mappingOperation.Success)
            {
                if (_keyMappingsHandler.KeyMappings.ContainsKey(_mappingOperation.SourceKey))
                {
                    int alreadyMappedKey = _keyMappingsHandler.KeyMappings[_mappingOperation.SourceKey];

                    if (alreadyMappedKey == _mappingOperation.MappedKey)
                    {
                        MessageBox.Show(
                            "Mapping with the provided keys already exists. Doing nothing",
                            "Information",
                            MessageBoxButton.OK, MessageBoxImage.Information
                        );
                        return;
                    }

                    var confirmation = MessageBox.Show(
                        $"Already have a mapping for the `{_mappingOperation.SourceKey}` key (currently mapped to `{alreadyMappedKey}`). Do you want to remap it?",
                        "Confirmation",
                        MessageBoxButton.YesNo, MessageBoxImage.Question
                    );

                    if (confirmation == MessageBoxResult.No)
                        return;

                    _keyMappingsHandler.RemoveMapping(_mappingOperation.SourceKey);
                    KeyMappings.Remove(KeyMappings.First(it => it.SourceKey == _mappingOperation.SourceKey));
                }

                _keyMappingsHandler.SetMapping(_mappingOperation.SourceKey, _mappingOperation.MappedKey);
                KeyMappings.Add(new KeyToKeyViewModel {SourceKey = _mappingOperation.SourceKey, MappedKey = _mappingOperation.MappedKey});
            }
        }

        private void ClearMappings_Execute()
        {
            _keyMappingsHandler.RemoveAllMappings();
            KeyMappings.Clear();
        }

        private void ImportMappings_Execute()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Exported mappings (*.json)|*.json",
                CheckFileExists = true
            };

            if (dialog.ShowDialog() != true)
                return;

            if (KeyMappings.Count > 0)
            {
                var confirmation = MessageBox.Show(
                    "Existing mappings will be cleared. Want to continue?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (confirmation == MessageBoxResult.No)
                    return;
            }

            var exportedModel = JsonConvert.DeserializeObject<ExportedMappings>(File.ReadAllText(dialog.FileName, Encoding.UTF8));

            _keyMappingsHandler.RemoveAllMappings();
            KeyMappings.Clear();

            if (exportedModel.KeyMappings == null)
                return;

            foreach (var mapping in exportedModel.KeyMappings)
            {
                _keyMappingsHandler.SetMapping(mapping.Key, mapping.Value);
                KeyMappings.Add(new KeyToKeyViewModel {SourceKey = mapping.Key, MappedKey = mapping.Value});
            }
        }

        private void ExportMappings_Execute()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Exported mappings (*.json)|*.json"
            };

            if (dialog.ShowDialog() != true)
                return;

            var exportedModel = new ExportedMappings
            {
                KeyMappings = KeyMappings.ToDictionary(it => it.SourceKey, it => it.MappedKey)
            };

            var fileDir = Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);

            File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(exportedModel, Formatting.Indented), Encoding.UTF8);
        }

        #endregion

        private bool GetStartupValue()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, false);

            if (rk == null)
            {
                MessageBox.Show("Can't read from registry", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return rk.GetValue(Resources.AppTitle, null) as string == _appUtils.GetExecutablePath();
        }

        private void SetStartupValue(bool enable)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true);

            if (rk == null)
            {
                MessageBox.Show("Can't write to registry", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (enable)
                rk.SetValue(Resources.AppTitle, _appUtils.GetExecutablePath());
            else
                rk.DeleteValue(Resources.AppTitle, false);
        }

        #region Property observers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(StartWithWindows):
                    SetStartupValue(StartWithWindows);
                    break;
                case nameof(SelectedKey):
                    DeleteMappingCommand.RaiseCanExecuteChanged();
                    break;
            }
        }

        private void ThemingUtils_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IThemingUtils.CurrentTheme):
                    OnPropertyChanged(nameof(AppTheme));
                    break;
            }
        }

        private void AppSettings_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IAppSettings.StartMinimized):
                    OnPropertyChanged(nameof(StartMinimized));
                    break;
            }
        }

        #endregion
    }
}

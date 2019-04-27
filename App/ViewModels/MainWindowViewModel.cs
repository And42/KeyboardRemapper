﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using App.Annotations;
using App.Logic;
using App.Logic.Operations;
using App.Logic.Utils;
using App.Properties;
using App.Windows;
using Microsoft.Win32;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;

namespace App.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private const string StartupRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public ObservableCollection<KeyToKeyViewModel> KeyMappings { get; } = new ObservableCollection<KeyToKeyViewModel>();

        public bool StartWithWindows
        {
            get => _startWithWindows;
            set => SetProperty(ref _startWithWindows, value);
        }

        public IActionCommand AddMappingCommand { get; }
        public IActionCommand ClearMappingsCommand { get; }

        [NotNull] private readonly KeyMappingsHandler _keyMappingsHandler;
        [NotNull] private readonly Provider<NewMappingWindow> _newMappingWindowProvider;
        [NotNull] private readonly MappingOperation _mappingOperation;
        [NotNull] private readonly AppUtils _appUtils;

        private bool _startWithWindows;

        public MainWindowViewModel(
            [NotNull] KeyMappingsHandler keyMappingsHandler,
            [NotNull] Provider<NewMappingWindow> newMappingWindowProvider,
            [NotNull] MappingOperation mappingOperation,
            [NotNull] AppUtils appUtils
        )
        {
            _keyMappingsHandler = keyMappingsHandler;
            _newMappingWindowProvider = newMappingWindowProvider;
            _mappingOperation = mappingOperation;
            _appUtils = appUtils;

            foreach (var mapping in keyMappingsHandler.KeyMappings)
                KeyMappings.Add(new KeyToKeyViewModel {SourceKey = mapping.Key, MappedKey = mapping.Value});

            AddMappingCommand = new ActionCommand(AddMapping_Execute);
            ClearMappingsCommand = new ActionCommand(ClearMappings_Execute);
            _startWithWindows = GetStartupValue();

            PropertyChanged += OnPropertyChanged;
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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(StartWithWindows):
                    SetStartupValue(StartWithWindows);
                    break;
            }
        }
    }
}

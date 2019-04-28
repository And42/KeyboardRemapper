using System;
using System.Windows;
using System.Windows.Controls;
using App.Interfaces.Logic;
using App.Interfaces.ViewModels;
using JetBrains.Annotations;

namespace App.Windows
{
    public partial class NewMappingWindow
    {
        [NotNull] private readonly INewMappingWindowViewModel _viewModel;
        [NotNull] private readonly IHooksHandler _hooksHandler;

        public NewMappingWindow(
            [NotNull] INewMappingWindowViewModel viewModel,
            [NotNull] IHooksHandler hooksHandler
        )
        {
            _viewModel = viewModel;
            _hooksHandler = hooksHandler;
            DataContext = viewModel;
            InitializeComponent();

            hooksHandler.SetAllKeysHandler(AllKeysHandler);
        }

        private bool AllKeysHandler(int keyCode)
        {
            if (!_viewModel.RecordKeyCommand.CanExecute(keyCode))
                return false;

            _viewModel.RecordKeyCommand.Execute(keyCode);
            return true;
        } 

        private void Apply_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ApplyCommand.CanExecute(null))
            {
                _viewModel.ApplyCommand.Execute();
                Close();
            }
        }

        private void Window_OnClosed(object sender, EventArgs e)
        {
            _hooksHandler.SetAllKeysHandler(null);
        }

        private void SourceKeyBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedKey = ((ComboBox) sender).SelectedItem as int?;
            if (selectedKey == null)
                return;

            _viewModel.SetSourceKeyCommand.Execute(selectedKey);
        }

        private void MappedKeyBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedKey = ((ComboBox)sender).SelectedItem as int?;
            if (selectedKey == null)
                return;

            _viewModel.SetMappedKeyCommand.Execute(selectedKey);
        }
    }
}

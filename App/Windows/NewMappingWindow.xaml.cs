using System;
using System.Windows;
using App.Annotations;
using App.Logic;
using App.ViewModels;

namespace App.Windows
{
    public partial class NewMappingWindow
    {
        [NotNull] private readonly NewMappingWindowViewModel _viewModel;
        [NotNull] private readonly HooksHandler _hooksHandler;

        public NewMappingWindow(
            [NotNull] NewMappingWindowViewModel viewModel,
            [NotNull] HooksHandler hooksHandler
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
    }
}

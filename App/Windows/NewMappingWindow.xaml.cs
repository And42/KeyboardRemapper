using System.Windows;
using System.Windows.Input;
using App.Annotations;
using App.ViewModels;

namespace App.Windows
{
    public partial class NewMappingWindow
    {
        [NotNull] private readonly NewMappingWindowViewModel _viewModel;

        public NewMappingWindow([NotNull] NewMappingWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Apply_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ApplyCommand.CanExecute(null))
            {
                _viewModel.ApplyCommand.Execute();
                Close();
            }
        }

        private void Window_OnKeyDown(object sender, KeyEventArgs e)
        {
            int keyCode = KeyInterop.VirtualKeyFromKey(e.Key);
            
            if (_viewModel.RecordKeyCommand.CanExecute(keyCode))
                _viewModel.RecordKeyCommand.Execute(keyCode);
        }
    }
}

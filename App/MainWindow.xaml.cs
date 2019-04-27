using App.Annotations;
using App.ViewModels;

namespace App
{
    public partial class MainWindow
    {
        public MainWindow([NotNull] MainWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}

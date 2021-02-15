using System.Windows;

namespace OpticalTextSelector
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
            viewModel.window = this;
            viewModel.canvas = canvas;

            this.DataContext = viewModel;
        }
    }
}

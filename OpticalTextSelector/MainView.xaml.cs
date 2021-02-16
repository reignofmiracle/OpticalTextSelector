using System.Windows;

namespace OpticalTextSelector
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            var viewModel = new MainViewModel(this.canvas);

            this.DataContext = viewModel;
        }
    }
}

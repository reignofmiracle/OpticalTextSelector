using System.Windows;
using System.Windows.Input;

namespace OpticalTextSelector
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            var viewModel = new MainViewModel(this.canvas);

            this.DataContext = viewModel;

            this.TitleBar.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            };
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

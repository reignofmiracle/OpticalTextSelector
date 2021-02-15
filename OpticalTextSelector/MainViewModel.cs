using Reactive.Bindings;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace OpticalTextSelector
{
    public class MainViewModel
    {
        KeyboardHook keyboardHook = new KeyboardHook();

        bool isActive = false;

        public MainViewModel()
        {
            this.keyboardHook.events
                .Where(e => e != null && e.Key == Keys.W && e.Control && e.IsKeyDown)
                .Throttle(TimeSpan.FromMilliseconds(50))
                .ObserveOnDispatcher()
                .Subscribe(e =>
            {
                this.isActive = !this.isActive;

                if (this.isActive)
                {
                    this.Background.Value = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                }
                else
                {
                    this.Background.Value = new SolidColorBrush(Colors.Transparent);
                }
            });            
        }

        public Window window { get; set; }
        public Canvas canvas { get; set; }

        public ReactivePropertySlim<Brush> Background { get; } = new ReactivePropertySlim<Brush>();
    }
}

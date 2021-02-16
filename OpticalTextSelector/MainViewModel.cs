using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpticalTextSelector
{
    public class MainViewModel
    {
        TesseractManager tesseractManager;
        SelectionManager selectionManager;

        KeyboardHook keyboardHook = new KeyboardHook();

        bool isActive = false;

        List<Point> selectionArea = new List<Point>();

        public MainViewModel(Canvas canvas)
        {
            this.tesseractManager = new TesseractManager("tessdata");
            this.selectionManager = new SelectionManager(canvas);

            this.keyboardHook.events
                .Where(e => e != null && e.Key == System.Windows.Forms.Keys.W && e.Control && e.IsKeyDown)
                .Throttle(TimeSpan.FromMilliseconds(50))
                .ObserveOnDispatcher()
                .Subscribe(e =>
            {
                this.isActive = !this.isActive;

                if (this.isActive)
                {
                    //this.Background.Value = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));

                    if (this.tesseractManager.Process())
                    {
                        this.ImageSource.Value = this.tesseractManager.BitmapImage;

                        this.selectionManager.Reset(this.tesseractManager.WordDictionary);
                    }
                }
                else
                {
                    this.Background.Value = new SolidColorBrush(Colors.Transparent);

                    this.ImageSource.Value = null;
                }
            });

            Observable.Merge(
                this.MouseLeftButtonUpCommand.Select(v => false),
                this.MouseLeftButtonDownCommand.Select(v => true))
                .CombineLatest(this.MouseMoveCommand.Cast<MouseEventArgs>())
                .Where(v => v.First)
                .Do(v => this.selectionArea.Add(v.Second.GetPosition(v.Second.Source as IInputElement)))
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOnDispatcher()
                .Subscribe(_ =>
                {
                    var source = this.selectionArea;
                    this.selectionArea = new List<Point>();

                    var rect = union(source);
                    this.selectionManager.Select(rect);
                });
        }

        public ReactivePropertySlim<Brush> Background { get; } = new ReactivePropertySlim<Brush>();
        public ReactivePropertySlim<ImageSource> ImageSource { get; } = new ReactivePropertySlim<ImageSource>();

        public ReactiveCommand MouseMoveCommand { get; } = new ReactiveCommand();
        public ReactiveCommand MouseLeftButtonUpCommand { get; } = new ReactiveCommand();
        public ReactiveCommand MouseLeftButtonDownCommand { get; } = new ReactiveCommand();

        private Rect union(List<Point> selectionArea)
        {
            var first = selectionArea[0];
            var rect = new Rect(first.X, first.Y, 0, 0);

            foreach (var item in selectionArea)
            {
                if (rect.Contains(item.X, item.Y))
                {
                    continue;
                }

                rect.Union(new Rect(item.X, item.Y, 0, 0));
            }

            return rect;
        }
    }
}

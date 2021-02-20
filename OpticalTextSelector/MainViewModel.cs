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

        bool isMouseLeftButtonDown;
        Point? mouseLeftButtonDownPosition;
        Rect? selectionRect;

        bool isSnapshot = false;

        SolidColorBrush windowResetBackgroundColor = new SolidColorBrush(Colors.Transparent);
        SolidColorBrush windowSnapshotBackgroundColor = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));

        SolidColorBrush buttonTrueBackgroundColor = new SolidColorBrush(Color.FromRgb(134, 95, 197));
        SolidColorBrush buttonTrueForegroundColor = new SolidColorBrush(Colors.White);

        SolidColorBrush buttonFalseBackgroundColor = new SolidColorBrush(Colors.White);
        SolidColorBrush buttonFalseForegroundColor = new SolidColorBrush(Colors.Black);

        public MainViewModel(Canvas canvas)
        {
            this.tesseractManager = new TesseractManager(canvas, "tessdata");
            this.selectionManager = new SelectionManager(canvas);

            this.MouseLeftButtonDownCommand
                .Subscribe(v =>
                {
                    this.isMouseLeftButtonDown = true;

                    if (mouseLeftButtonDownPosition == null)
                    {
                        mouseLeftButtonDownPosition = v.GetPosition(v.Source as IInputElement);
                    }
                });

            this.MouseLeftButtonUpCommand
                .Subscribe(v =>
                {
                    if (this.selectionRect != null)
                    {
                        this.selectionManager.Select((Rect)this.selectionRect);
                    }

                    this.isMouseLeftButtonDown = false;

                    this.mouseLeftButtonDownPosition = null;

                    this.selectionRect = null;
                });

            this.MouseMoveCommand
                .Where(_ => this.isSnapshot && this.isMouseLeftButtonDown)
                .Subscribe(v =>
                {
                    var startPosition = (Point)mouseLeftButtonDownPosition;
                    var endPosition = v.GetPosition(v.Source as IInputElement);

                    this.selectionRect = GetRect(startPosition, endPosition);
                    this.selectionManager.UpdateSelectionBox((Rect)this.selectionRect);                    
                });
            
            this.ResetCommand.Subscribe(Reset);
            this.SnapshotCommand.Subscribe(Snapshot);            

            SetColors(true);
        }

        public ReactivePropertySlim<Brush> WindowBackground { get; } = new ReactivePropertySlim<Brush>();
        public ReactivePropertySlim<ImageSource> ImageSource { get; } = new ReactivePropertySlim<ImageSource>();

        public ReactiveCommand<MouseEventArgs> MouseMoveCommand { get; } = new ReactiveCommand<MouseEventArgs>();
        public ReactiveCommand<MouseButtonEventArgs> MouseLeftButtonUpCommand { get; } = new ReactiveCommand<MouseButtonEventArgs>();
        public ReactiveCommand<MouseButtonEventArgs> MouseLeftButtonDownCommand { get; } = new ReactiveCommand<MouseButtonEventArgs>();

        public ReactivePropertySlim<Brush> ResetBackground { get; } = new ReactivePropertySlim<Brush>();
        public ReactivePropertySlim<Brush> ResetForeground { get; } = new ReactivePropertySlim<Brush>();
        public ReactiveCommand ResetCommand { get; } = new ReactiveCommand();
        public ReactivePropertySlim<Brush> SnapshotBackground { get; } = new ReactivePropertySlim<Brush>();
        public ReactivePropertySlim<Brush> SnapshotForeground { get; } = new ReactivePropertySlim<Brush>();
        public ReactiveCommand SnapshotCommand { get; } = new ReactiveCommand();

        private Rect GetRect(Point start, Point end)
        {
            var left = Math.Min(start.X, end.X);
            var top = Math.Min(start.Y, end.Y);
            var width = Math.Abs(start.X - end.X);
            var height = Math.Abs(start.Y - end.Y);
            return new Rect(left, top, width, height);
        }

        private void Reset()
        {
            this.isSnapshot = false;

            SetColors(true);

            this.ImageSource.Value = null;
            this.selectionManager.Clear();
        }

        private void Snapshot()
        {           
            SetColors(false);

            if (this.tesseractManager.Process())
            {
                this.ImageSource.Value = this.tesseractManager.BitmapImage;
                this.selectionManager.Reset(this.tesseractManager.ResultWords);
            }

            this.isSnapshot = true;
        }

        private void SetColors(bool isReset)
        {
            if (isReset)
            {
                this.WindowBackground.Value = this.windowResetBackgroundColor;

                this.ResetBackground.Value = this.buttonTrueBackgroundColor;
                this.ResetForeground.Value = this.buttonTrueForegroundColor;

                this.SnapshotBackground.Value = this.buttonFalseBackgroundColor;
                this.SnapshotForeground.Value = this.buttonFalseForegroundColor;
            } 
            else
            {
                this.WindowBackground.Value = this.windowSnapshotBackgroundColor;

                this.ResetBackground.Value = this.buttonFalseBackgroundColor;
                this.ResetForeground.Value = this.buttonFalseForegroundColor;

                this.SnapshotBackground.Value = this.buttonTrueBackgroundColor;
                this.SnapshotForeground.Value = this.buttonTrueForegroundColor;
            }
        }
    }
}

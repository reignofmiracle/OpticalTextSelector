using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpticalTextSelector
{
    public class SelectionManager
    {
        Canvas canvas;

        List<ResultWord> resultWords;

        System.Windows.Shapes.Rectangle selectionBox;

        public SelectionManager(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void Clear()
        {
            this.canvas.Children.Clear();

            this.selectionBox = null;
        }

        public void UpdateSelectionBox(Rect rect)
        {
            if (selectionBox == null)
            {
                this.selectionBox = new System.Windows.Shapes.Rectangle();
                this.selectionBox.Width = rect.Width;
                this.selectionBox.Height = rect.Height;
                this.selectionBox.Stroke = new SolidColorBrush(Colors.Red);
                this.selectionBox.StrokeThickness = 1;
                this.canvas.Children.Add(this.selectionBox);

                Canvas.SetLeft(this.selectionBox, rect.X);
                Canvas.SetTop(this.selectionBox, rect.Y);
            }
            else
            {
                this.selectionBox.Width = rect.Width;
                this.selectionBox.Height = rect.Height;

                Canvas.SetLeft(this.selectionBox, rect.X);
                Canvas.SetTop(this.selectionBox, rect.Y);
            }
        }

        public void Reset(List<ResultWord> resultWords)
        {
            if (resultWords == null)
            {
                return;
            }

            this.resultWords = resultWords;

            this.canvas.Children.Clear();
        }

        public void Select(Rect rect)
        {
            Clear();

            UpdateSelectionBox(rect);

            List<ResultWord> selectedWords = new List<ResultWord>();

            foreach (var item in this.resultWords)
            {
                if (rect.IntersectsWith(item.BoundingBox))
                {
                    selectedWords.Add(item);                    
                }
            }

            foreach (var item in selectedWords)
            {
                AddSelection(item.BoundingBox);
            }

            Clipboard.SetText(this.BuildSelectionString(selectedWords));
        }

        private System.Windows.Shapes.Rectangle AddSelection(Rect rect)
        {
            var rectangle = new System.Windows.Shapes.Rectangle();
            rectangle.Width = rect.Width;
            rectangle.Height = rect.Height;
            rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 255, 0, 0));
            
            this.canvas.Children.Add(rectangle);
            Canvas.SetLeft(rectangle, rect.X);
            Canvas.SetTop(rectangle, rect.Y);

            return rectangle;
        }

        private string BuildSelectionString(List<ResultWord> resultWords)
        {
            Rect baseRect = Rect.Empty;

            var result = new List<string>();

            foreach (var item in resultWords)
            {
                if (baseRect == Rect.Empty ||
                    Rect.Intersect((Rect)baseRect, item.BoundingBox) == Rect.Empty)
                {
                    baseRect = new Rect(item.BoundingBox.X, item.BoundingBox.Y, double.MaxValue, item.BoundingBox.Height);
                    result.Add("\n");
                }
                else
                {
                    result.Add(" ");
                }

                result.Add(item.Text);
            }

            return string.Join("", result).Trim();
        }
    }
}

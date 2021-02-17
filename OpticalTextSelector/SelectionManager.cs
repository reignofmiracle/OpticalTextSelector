using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpticalTextSelector
{
    public class SelectionManager
    {
        Canvas canvas;

        Dictionary<Rect, string> wordDictionary;

        public SelectionManager(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void Reset(Dictionary<Rect, string> wordDictionary)
        {
            if (wordDictionary == null)
            {
                return;
            }

            this.wordDictionary = wordDictionary;

            this.canvas.Children.Clear();
        }

        public void Select(Rect rect)
        {
            this.canvas.Children.Clear();

            foreach (var item in this.wordDictionary)
            {
                if (rect.IntersectsWith(item.Key))
                {
                    var selection = createSelection(item.Key);
                    this.canvas.Children.Add(selection);
                    Canvas.SetLeft(selection, item.Key.X);
                    Canvas.SetTop(selection, item.Key.Y + (item.Key.Height / 2));

                    Debug.WriteLine($"{item.Value}, {rect}");
                }
            }
        }

        private System.Windows.Shapes.Rectangle createSelection(Rect rect)
        {
            var selection = new System.Windows.Shapes.Rectangle();
            selection.Width = rect.Width;
            selection.Height = rect.Height;
            //selection.Stroke = System.Windows.Media.Brushes.Red;
            //selection.StrokeThickness = 1;
            selection.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(125, 255, 0, 0));
            return selection;
        }
    }
}

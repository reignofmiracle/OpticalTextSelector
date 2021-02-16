using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

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
                if (rect.Contains(item.Key))
                {
                    var selection = createSelection(item.Key);
                    this.canvas.Children.Add(selection);
                    Canvas.SetTop(selection, item.Key.X);
                    Canvas.SetLeft(selection, item.Key.Y);
                }
            }
        }

        private System.Windows.Shapes.Rectangle createSelection(Rect rect)
        {
            var selection = new System.Windows.Shapes.Rectangle();
            selection.Width = rect.Width;
            selection.Height = rect.Height;
            selection.Stroke = System.Windows.Media.Brushes.Red;
            selection.StrokeThickness = 1;
            return selection;
        }
    }
}

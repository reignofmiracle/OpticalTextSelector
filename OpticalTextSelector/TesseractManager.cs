using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Tesseract;

namespace OpticalTextSelector
{
    public class TesseractManager
    {
        Canvas canvas;
        TesseractEngine tesseractEngine;

        public TesseractManager(Canvas canvas, string tessdata)
        {
            this.canvas = canvas;
            this.tesseractEngine = new TesseractEngine(tessdata, "eng", EngineMode.Default);
        }

        public BitmapImage BitmapImage { get; private set; }

        public List<ResultWord> ResultWords { get; private set; }

        public bool Process()
        {
            try 
            {
                var bitmap = getBitmap();

                this.BitmapImage = bitmap.Convert();

                UpdateSelection(bitmap);

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }            
        }

        private Bitmap getBitmap()
        {
            try
            {
                var position = this.canvas.PointToScreen(new System.Windows.Point(0, 0));
                var left = (int)position.X;
                var top = (int)position.Y;
                var width = (int)this.canvas.ActualWidth;
                var height = (int)this.canvas.ActualHeight;
                var bounds = Screen.AllScreens[0].Bounds;
                var bitmap = new Bitmap(width, height);
                var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(left, top, 0, 0, new Size(width, height));
                return bitmap;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        private void UpdateSelection(Bitmap bitmap)
        {
            using (var img = PixConverter.ToPix(bitmap))
            {
                using (var page = tesseractEngine.Process(img))
                {
                    var pageIteratorLevel = PageIteratorLevel.Word;

                    var resultWords = new List<ResultWord>();

                    using (var iter = page.GetIterator())
                    {
                        iter.Begin();

                        do
                        {
                            var text = iter.GetText(pageIteratorLevel).Trim();
                            if (text.Length == 0)
                            {
                                continue;
                            }

                            Rect boundingBox;
                            if (iter.TryGetBoundingBox(pageIteratorLevel, out boundingBox))
                            {
                                var result = new ResultWord();
                                result.Text = text;
                                result.BoundingBox = Convert(boundingBox);
                                resultWords.Add(result);
                            }
                        } while (iter.Next(PageIteratorLevel.Word));
                    }

                    this.ResultWords = resultWords;
                }
            }
        }

        private System.Windows.Rect Convert(Rect rect)
        {
            return new System.Windows.Rect(rect.X1, rect.Y1, rect.Width, rect.Height);
        }        
    }
    public class ResultWord
    {
        public string Text { get; set; }
        public System.Windows.Rect BoundingBox { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Tesseract;

namespace OpticalTextSelector
{
    public class TesseractManager
    {
        TesseractEngine tesseractEngine;

        public TesseractManager(string tessdata)
        {
            this.tesseractEngine = new TesseractEngine(tessdata, "eng", EngineMode.Default);
        }

        public BitmapImage BitmapImage { get; private set; }

        public Dictionary<System.Windows.Rect, string> WordDictionary { get; private set; }

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
                var bounds = Screen.AllScreens[0].Bounds;
                var bitmap = new Bitmap(bounds.Width, bounds.Height);
                var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);
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

                    var wordDictionary = new Dictionary<System.Windows.Rect, string>();

                    using (var iter = page.GetIterator())
                    {
                        iter.Begin();

                        do
                        {
                            var content = iter.GetText(pageIteratorLevel);

                            Rect boundingBox;
                            if (iter.TryGetBoundingBox(pageIteratorLevel, out boundingBox))
                            {
                                wordDictionary.Add(Convert(boundingBox), content);
                            }
                        } while (iter.Next(PageIteratorLevel.Word));
                    }

                    this.WordDictionary = wordDictionary;
                }
            }
        }

        private System.Windows.Rect Convert(Rect rect)
        {
            return new System.Windows.Rect(rect.X1, rect.Y1, rect.Width, rect.Height);
        }
    }
}

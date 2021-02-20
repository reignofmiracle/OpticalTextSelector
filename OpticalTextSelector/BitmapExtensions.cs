using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace OpticalTextSelector
{
    public static class BitmapExtensions
    {
        public static BitmapImage Convert(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public static Bitmap ConvertToGrayscale(this Bitmap source)
        {
            Bitmap ret = new Bitmap(source.Width, source.Height);
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    var c = source.GetPixel(x, y);
                    var value = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    ret.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }

            return ret;
        }
    }
}

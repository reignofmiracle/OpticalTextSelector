using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tesseract;

namespace OpticalTextSelectorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new TesseractEngine(@"../../../../testdata/tessdata", "eng", EngineMode.Default);
            var img = Pix.LoadFromFile(@"../../../../testdata/sample.png");
            var page = engine.Process(img);
            
            List<Tuple<string, Rect?>> contentList = new List<Tuple<string, Rect?>>();

            var pageIteratorLevel = PageIteratorLevel.Word;

            using (var iter = page.GetIterator())
            {
                iter.Begin();

                do
                {
                    var content = iter.GetText(pageIteratorLevel);

                    Rect boundingBox;
                    if (iter.TryGetBoundingBox(pageIteratorLevel, out boundingBox))
                    {
                        contentList.Add(Tuple.Create<string, Rect?>(content, boundingBox));
                    } else
                    {
                        contentList.Add(Tuple.Create<string, Rect?>(content, null));
                    }

                } while (iter.Next(PageIteratorLevel.Word));
            }

            foreach (var item in contentList)
            {
                Console.WriteLine(item);
            }
        }
    }
}

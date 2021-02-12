using NUnit.Framework;
using System.Diagnostics;
using Tesseract;

namespace OpticalTextSelectorTest.unittest
{
    [TestFixture]
    public class usage_test
    {
        [Test]
        public void TestEngine()
        {
            var engine = new TesseractEngine(@"../../../testdata/tessdata", "eng", EngineMode.Default);
            var img = Pix.LoadFromFile(@"../../../testdata/sample.png");
            var page = engine.Process(img);
            var text = page.GetText();
            Debug.WriteLine(text);
        }
    }
}

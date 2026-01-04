using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CsMlImageProcess
{
    public static class ImageUtils
    {
        public static Bitmap GenerateTestImage(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Fill background
                g.Clear(Color.White);

                // Draw some shapes
                g.FillRectangle(Brushes.Red, 50, 50, 100, 100);
                g.FillEllipse(Brushes.Blue, 150, 50, 100, 100);
                g.FillRectangle(Brushes.Green, 100, 150, 150, 50);
                
                // Add some noise or lines for edge detection
                Pen blackPen = new Pen(Color.Black, 3);
                g.DrawLine(blackPen, 0, 0, width, height);
                g.DrawLine(blackPen, width, 0, 0, height);
            }
            return bmp;
        }

        public static Bitmap LoadImage(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Image not found: {path}");
            }
            return new Bitmap(path);
        }

        public static void SaveImage(Bitmap bmp, string path)
        {
            bmp.Save(path, ImageFormat.Png);
            Console.WriteLine($"Image saved to: {path}");
        }
    }
}

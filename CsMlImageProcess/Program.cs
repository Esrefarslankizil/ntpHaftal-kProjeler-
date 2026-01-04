using System;
using System.Drawing;
using System.IO;

namespace CsMlImageProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== C# ML & Math Image Processing Demo ===");
            
            // 1. Generate/Load Image
            Console.WriteLine("1. Generating Test Image...");
            string inputPath = Path.Combine(Environment.CurrentDirectory, "test_image.png");
            Bitmap original = ImageUtils.GenerateTestImage(400, 300);
            ImageUtils.SaveImage(original, inputPath);

            // 2. Math Processing (Sobel)
            Console.WriteLine("\n2. Running Math Processing (Sobel Edge Detection)...");
            Bitmap edgeImage = MathProcessor.ApplySobelEdgeDetection(original);
            string sobelPath = Path.Combine(Environment.CurrentDirectory, "output_sobel.png");
            ImageUtils.SaveImage(edgeImage, sobelPath);

            // 3. ML Processing (K-Means)
            Console.WriteLine("\n3. Running ML Processing (K-Means Segmentation)...");
            Bitmap segmentedImage = MLProcessor.ApplyKMeansSegmentation(original, numberOfClusters: 4);
            string kmeansPath = Path.Combine(Environment.CurrentDirectory, "output_kmeans.png");
            ImageUtils.SaveImage(segmentedImage, kmeansPath);

            Console.WriteLine("\nDone! Outputs saved.");
            Console.WriteLine($"Original: {inputPath}");
            Console.WriteLine($"Sobel: {sobelPath}");
            Console.WriteLine($"K-Means: {kmeansPath}");
        }
    }
}

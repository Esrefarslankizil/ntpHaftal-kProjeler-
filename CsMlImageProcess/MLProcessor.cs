using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace CsMlImageProcess
{
    public class PixelData
    {
        [LoadColumn(0)] public float R;
        [LoadColumn(1)] public float G;
        [LoadColumn(2)] public float B;
    }

    public class ClusterPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId;

        [ColumnName("Score")]
        public float[] Distances;
    }

    public static class MLProcessor
    {
        public static Bitmap ApplyKMeansSegmentation(Bitmap original, int numberOfClusters)
        {
            var mlContext = new MLContext(seed: 0);
            int width = original.Width;
            int height = original.Height;

            Console.WriteLine("Preparing data for ML...");
            List<PixelData> pixels = new List<PixelData>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = original.GetPixel(x, y);
                    pixels.Add(new PixelData { R = c.R, G = c.G, B = c.B });
                }
            }

            IDataView dataView = mlContext.Data.LoadFromEnumerable(pixels);

            // Define trainer options
            var options = new Microsoft.ML.Trainers.KMeansTrainer.Options
            {
                NumberOfClusters = numberOfClusters,
                FeatureColumnName = "Features" // Default name expected by K-Means
            };

            // Build pipeline
            var pipeline = mlContext.Transforms.Concatenate("Features", "R", "G", "B")
                .Append(mlContext.Clustering.Trainers.KMeans(options));

            Console.WriteLine("Training K-Means model...");
            var model = pipeline.Fit(dataView);

            Console.WriteLine("Predicting clusters...");
            var predictor = mlContext.Model.CreatePredictionEngine<PixelData, ClusterPrediction>(model);

            // Reconstruct segmented image
            Bitmap segmented = new Bitmap(width, height);
            
            // Get centroids to color the segments with the average color (optional, or just random colors)
            // For simplicity, we can just use predefined colors for clusters or the centroid value if we could easily extract it.
            // Let's use specific distinct colors for visualization.
            Color[] clusterColors = new Color[] 
            {
                Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Cyan, Color.Magenta, Color.Orange, Color.Purple
            };

            int pixelIndex = 0;
            // To speed up, we can batch predict or just loop. Looping is fine for small demo.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var p = pixels[pixelIndex++];
                    var prediction = predictor.Predict(p);
                    
                    // Maps 1..N to 0..N-1 index
                    int colorIndex = (int)(prediction.PredictedClusterId - 1) % clusterColors.Length;
                    segmented.SetPixel(x, y, clusterColors[colorIndex]);
                }
            }

            return segmented;
        }
    }
}

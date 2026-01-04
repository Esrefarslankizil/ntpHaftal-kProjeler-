using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CsMlImageProcess
{
    public static class MathProcessor
    {
        public static Bitmap ApplySobelEdgeDetection(Bitmap original)
        {
            int width = original.Width;
            int height = original.Height;
            Bitmap edgeBitmap = new Bitmap(width, height);

            // Accessing pixel data conveniently (this is slow but clear for demo)
            // For production, LockBits is preferred.
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    // Sobel Kernels
                    // Gx      Gy
                    // -1 0 1  -1 -2 -1
                    // -2 0 2   0  0  0
                    // -1 0 1   1  2  1

                    int gx = 0;
                    int gy = 0;

                    // Kernel Loop
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            Color pixel = original.GetPixel(x + kx, y + ky);
                            int gray = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);

                            // Initializing Kernels
                            int valX = 0;
                            int valY = 0;

                            // X Gradient Kernel
                            if (kx == -1) valX = -1;
                            else if (kx == 1) valX = 1;
                            if (ky == 0) valX *= 2; // Center row weight

                             // Y Gradient Kernel
                            if (ky == -1) valY = -1;
                            else if (ky == 1) valY = 1;
                            if (kx == 0) valY *= 2; // Center col weight

                            gx += valX * gray;
                            gy += valY * gray;
                        }
                    }

                    int magnitude = (int)Math.Sqrt(gx * gx + gy * gy);
                    
                    // Clamp to 255
                    magnitude = Math.Min(255, Math.Max(0, magnitude));
                    
                    // Invert so edges are black, background white (optional, but standard usually white edges on black)
                    // Let's do standard: high magnitude = white edge
                    edgeBitmap.SetPixel(x, y, Color.FromArgb(magnitude, magnitude, magnitude));
                }
            }
            return edgeBitmap;
        }
    }
}

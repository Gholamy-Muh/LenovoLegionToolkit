﻿using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class SpectrumScreenCapture : SpectrumKeyboardBacklightController.IScreenCapture
    {
        private const PixelFormat PIXEL_FORMAT = PixelFormat.Format16bppRgb555;
        private const int CUT_OFF = 32;

        public RGBColor[][] CaptureScreen(int width, int height)
        {
            var screen = Screen.PrimaryScreen.Bounds;

            using var targetImage = new Bitmap(width, height, PIXEL_FORMAT);

            using (var image = new Bitmap(screen.Width, screen.Height, PIXEL_FORMAT))
            {
                using (var graphics = Graphics.FromImage(image))
                    graphics.CopyFromScreen(screen.Left, screen.Top, 0, 0, screen.Size);

                using var targetGraphics = Graphics.FromImage(targetImage);
                targetGraphics.Clear(Color.Black);
                targetGraphics.DrawImage(image, new Rectangle(0, 0, width, height));
            }

            var colors = new RGBColor[height][];
            for (var y = 0; y < height; y++)
            {
                colors[y] = new RGBColor[width];
                for (var x = 0; x < width; x++)
                {
                    var pixel = targetImage.GetPixel(x, y);

                    if (pixel.R < CUT_OFF && pixel.G < CUT_OFF && pixel.B < CUT_OFF)
                        colors[y][x] = new RGBColor(0, 0, 0);
                    else
                        colors[y][x] = new RGBColor(pixel.R, pixel.G, pixel.B);
                }
            }

            return colors;
        }
    }
}

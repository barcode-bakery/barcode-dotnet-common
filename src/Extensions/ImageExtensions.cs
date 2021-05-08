using SkiaSharp;

namespace BarcodeBakery.Common.Extensions
{
    /// <summary>
    /// Extensions on the <see cref="BCGSurface"/>.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Creates a filled rectangle.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x1">X1.</param>
        /// <param name="y1">Y1.</param>
        /// <param name="x2">X2.</param>
        /// <param name="y2">Y2.</param>
        /// <param name="color">The color.</param>
        public static void FillRectangle(this BCGSurface image, int x1, int y1, int x2, int y2, SKColor color)
        {
            image.SKSurface.Canvas.DrawRect(new SKRect(x1, y1, x2 + 1, y2 + 1), new SKPaint() { Style = SKPaintStyle.Fill, Color = color });
        }

        /// <summary>
        /// Creates a filled circle.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="color">The color.</param>
        public static void FillCircle(this BCGSurface image, int x, int y, int radius, SKColor color)
        {
            image.SKSurface.Canvas.DrawCircle(x, y, radius, new SKPaint() { Style = SKPaintStyle.Fill, Color = color });
        }
    }
}

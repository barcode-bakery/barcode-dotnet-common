using SkiaSharp;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// The surface used for drawing.
    /// </summary>
    public class BCGSurface
    {
        /// <summary>
        /// Implementation details of the surface.
        /// </summary>
        public SKSurface SKSurface { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="surface">Implementation details of the surface.</param>
        public BCGSurface(SKSurface surface)
        {
            SKSurface = surface;
        }
    }
}
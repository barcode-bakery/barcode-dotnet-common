using SkiaSharp;
using System;
using System.Globalization;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// Holds Color in RGB Format.
    /// </summary>
    public class BCGColor
    {
        /// <summary>
        /// Hexadecimal value for R.
        /// </summary>
        protected int vR;

        /// <summary>
        /// Hexadecimal value for G.
        /// </summary>
        protected int vG;

        /// <summary>
        /// Hexadecimal value for B.
        /// </summary>
        protected int vB;

        /// <summary>
        /// Indicates if the color is considered transparent.
        /// </summary>
        protected bool transparent;

        /// <summary>
        /// Saves RGB value into the classes.
        /// Given 3 parameters int (R, G, B).
        /// </summary>
        /// <param name="r">Red.</param>
        /// <param name="g">Green.</param>
        /// <param name="b">Blue.</param>
        public BCGColor(int r, int g, int b)
        {
            this.vR = r;
            this.vG = g;
            this.vB = b;
        }

        /// <summary>
        /// Saves RGB value into the classes.
        /// Given 1 parameter string hex value (#ff0000) (preceding with #), or
        /// Given 1 parameter string color code(white, black, orange...)
        /// </summary>
        /// <param name="color">The color.</param>
        public BCGColor(string color)
        {
            if (!string.IsNullOrEmpty(color))
            {
                if (color.Length == 7 && color.StartsWith("#", StringComparison.Ordinal))
                {
                    this.vR = Int16.Parse(color.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture);
                    this.vG = Int16.Parse(color.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture);
                    this.vB = Int16.Parse(color.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture);
                }
                else
                {
                    ParseHex(BCGColor.GetColor(color));
                }
            }
            else
            {
                ParseHex(0);
            }
        }

        /// <summary>
        /// Saves RGB value into the classes.
        /// Given 1 parameter int hex value (0xff0000)
        /// </summary>
        /// <param name="hex">The hexadecimal value.</param>
        public BCGColor(int hex)
        {
            ParseHex(hex);
        }

        /// <summary>
        /// Saves RGB value into the classes.
        /// Given the implementation details SKColor.
        /// </summary>
        /// <param name="color">The color.</param>
        public BCGColor(SKColor color)
        {
            this.vR = color.Red;
            this.vG = color.Green;
            this.vB = color.Blue;
        }

        private void ParseHex(int hex)
        {
            this.vR = (hex & 0xff0000) >> 16;
            this.vG = (hex & 0x00ff00) >> 8;
            this.vB = (hex & 0x0000ff);
        }

        /// <summary>
        /// Sets the color transparent.
        /// </summary>
        /// <param name="transparent">Indicates if the color should be transparent.</param>
        public void SetTransparent(bool transparent)
        {
            this.transparent = transparent;
        }

        /// <summary>
        /// Returns red color.
        /// </summary>
        /// <returns>The red color.</returns>
        public int R()
        {
            return this.vR;
        }

        /// <summary>
        /// Returns green color.
        /// </summary>
        /// <returns>The green color.</returns>
        public int G()
        {
            return this.vG;
        }

        /// <summary>
        /// Returns blue color.
        /// </summary>
        /// <returns>The blue color.</returns>
        public int B()
        {
            return this.vB;
        }

        /// <summary>
        /// Creates a color to be used on the surface.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <returns>The color.</returns>
        public SKColor Allocate(BCGSurface image)
        {
            return new SKColor((byte)this.vR, (byte)this.vG, (byte)this.vB);
        }

        /// <summary>
        /// Returns class of <see cref="BCGColor"/> depending of the string color.
        ///
        /// If the color doens't exist, it defaults to white.
        /// </summary>
        /// <param name="color">The color name.</param>
        /// <returns>The color.</returns>
        public static int GetColor(string color)
        {
            return BCGColor.GetColor(color, "white");
        }

        /// <summary>
        /// Returns class of <see cref="BCGColor"/> depending of the string color.
        ///
        /// If the color doens't exist, it defaults to <paramref name="defaultColor"/>.
        /// </summary>
        /// <param name="color">The color name.</param>
        /// <param name="defaultColor">The default color name.</param>
        /// <returns>The color.</returns>
        public static int GetColor(string color, string defaultColor)
        {
            return (color.ToLower(CultureInfo.CurrentCulture)) switch
            {
                "" or "white" => 0xffffff,
                "black" => 0x000000,
                "maroon" => 0x800000,
                "red" => 0xff0000,
                "orange" => 0xffa500,
                "yellow" => 0xffff00,
                "olive" => 0x808000,
                "purple" => 0x800080,
                "fuchsia" => 0xff00ff,
                "lime" => 0x00ff00,
                "green" => 0x008000,
                "navy" => 0x000080,
                "blue" => 0x0000ff,
                "aqua" => 0x00ffff,
                "teal" => 0x008080,
                "silver" => 0xc0c0c0,
                "gray" => 0x808080,
                _ => BCGColor.GetColor(defaultColor, "white"),
            };
        }
    }
}

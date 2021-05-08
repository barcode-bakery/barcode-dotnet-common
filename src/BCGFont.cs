using SkiaSharp;
using System;
using System.IO;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// Holds font family and size.
    /// </summary>
    public class BCGFont : ICloneable, IDisposable
    {
        private SKPaint? font;
        private string text;
        private int rotationAngle = 0;
        private BCGColor foregroundColor;

        private SKRect? fontInfo = null;

        // TODO, handle the exception here, throw something ourselves if the font does not exist.
        /// <summary>
        /// Cosntructor.
        /// </summary>
        /// <param name="pathOrFontFamily">Font Family or path to a font family.</param>
        /// <param name="size">The text size.</param>
        public BCGFont(string pathOrFontFamily, int size)
        {
            var typeFace = SKTypeface.FromFamilyName(pathOrFontFamily);
            if (typeFace == null && File.Exists(pathOrFontFamily))
            {
                typeFace = SKTypeface.FromFile(pathOrFontFamily);
            }

            if (typeFace == null)
            {
                throw new Exception();
            }

            font = new SKPaint
            {
                Typeface = typeFace,
                TextSize = size,
                IsAntialias = true
            };
            foregroundColor = new BCGColor("black");
            text = string.Empty;
        }

        /**
         * Gets the text associated to the font.
         *
         * @return string
         */
        public string GetText()
        {
            return this.text;
        }

        /**
         * Sets the text associated to the font.
         *
         * @param string text
         */
        public void SetText(string text)
        {
            this.text = text;
            this.fontInfo = null;
        }

        /**
         * Gets the rotation in degree.
         *
         * @return int
         */
        public int GetRotationAngle()
        {
            return this.rotationAngle % 360;
        }

        /**
         * Sets the rotation in degree.
         *
         * @param int
         */
        public void SetRotationAngle(int rotationAngle)
        {
            this.rotationAngle = rotationAngle;

            if (this.rotationAngle != 90 && this.rotationAngle != 180 && this.rotationAngle != 270)
            {
                this.rotationAngle = 0;
            }

            this.rotationAngle %= 360;
            this.fontInfo = null;
        }

        /**
         * Gets the background color.
         *
         * @return BCGColor
         */
        public BCGColor? GetBackgroundColor()
        {
            return null;
        }

        /**
         * Sets the background color.
         *
         * @param BCGColor $backgroundColor
         */
        public void SetBackgroundColor(BCGColor backgroundColor)
        {
        }

        /**
         * Gets the foreground color.
         *
         * @return BCGColor
         */
        public BCGColor GetForegroundColor()
        {
            return this.foregroundColor;
        }

        /**
         * Sets the foreground color.
         *
         * @param BCGColor $foregroundColor
         */
        public void SetForegroundColor(BCGColor foregroundColor)
        {
            this.foregroundColor = foregroundColor;
        }

        /**
         * Returns the width and height that the text takes to be written.
         *
         * @return int[]
         */
        public int[] GetDimension()
        {
            var width = GetWidth();
            var height = GetHeight();
            var rotationAngle = this.GetRotationAngle();
            if (rotationAngle == 90 || rotationAngle == 270)
            {
                return new int[] { height, width };
            }
            else
            {
                return new int[] { width, height };
            }
        }

        /**
         * Draws the text on the image at a specific position.
         * $x and $y represent the left bottom corner.
         *
         * @param resource $im
         * @param int $x
         * @param int $y
         */
        public void Draw(BCGSurface image, int x, int y)
        {
            var width = GetWidth();
            var height = GetHeight();
            var top = GetFontInfo().Top;
            var rotationAngle = GetRotationAngle();

            font!.Color = this.foregroundColor.Allocate(image);

            var canvas = image.SKSurface.Canvas;
            canvas.Save();
            switch (rotationAngle)
            {
                case 0:
                    canvas.Translate(x, y - top);
                    break;
                case 90:
                    canvas.Translate(height + x + top, y);
                    break;
                case 180:
                    canvas.Translate(width + x, height + y + top);
                    break;
                case 270:
                    canvas.Translate(x - top, width + y);
                    break;
            }

            canvas.RotateDegrees(rotationAngle);


            image.SKSurface.Canvas.DrawText(text, 0, 0, font);
            canvas.Restore();
        }

        /// <summary>
        /// Gets the text height.
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            if (this.font == null)
            {
                return 0;
            }
            else
            {
                return (int)GetFontInfo().Height;
            }
        }

        /// <summary>
        /// Gets the text width.
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            if (this.font == null)
            {
                return 0;
            }
            else
            {
                var fontInfo = GetFontInfo();
                return (int)Math.Ceiling(fontInfo.Width) + (int)Math.Ceiling(fontInfo.Left);
            }
        }

        /// <summary>
        /// Gets the baseline size.
        /// </summary>
        /// <returns></returns>
        public int GetUnderBaseline()
        {
            /*
            if (this.font == null)
            {
                return 0;
            }
            else
            {
                return (int)this.font.Size * this.font.FontFamily.GetCellDescent(this.font.Style) / this.font.FontFamily.GetEmHeight(this.font.Style);
            }
            */
            return 0;
        }

        /// <summary>
        /// Clones the font.
        /// </summary>
        /// <returns>A new font object.</returns>
        public object Clone()
        {
            return new BCGFont(font!.Typeface.FamilyName, (int)font.TextSize);
        }

#region IDisposable Members

        /// <summary>
        /// Disposes the font.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);

        }

        /// <summary>
        /// Disposes the font.
        /// </summary>
        /// <param name="disposing">Is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.font != null)
                {
                    this.font = null;
                }
            }
        }

        private SKRect GetFontInfo()
        {
            if (!this.fontInfo.HasValue)
            {
                SKRect textSize = SKRect.Empty;
                font!.MeasureText(this.text, ref textSize);
                fontInfo = textSize;
            }

            return fontInfo.Value;
        }

        #endregion
    }
}
using BarcodeBakery.Common.Extensions;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// Base class for Barcode 1D and 2D.
    /// </summary>
    public abstract class BCGBarcode
    {
        /// <summary>
        /// Used for background color.
        /// </summary>
        protected const int COLOR_BG = 0;

        /// <summary>
        /// Used for foreground color.
        /// </summary>
        protected const int COLOR_FG = 1;

        /// <summary>
        /// Color for the foreground.
        /// </summary>
        protected BCGColor colorFg = null!; // !Assigned in the constructor.

        /// <summary>
        /// Color for the background.
        /// </summary>
        protected BCGColor colorBg = null!; // !Assigned in the constructor.

        /// <summary>
        /// Scale of the graphic, default: 1.
        /// </summary>
        protected int scale;

        /// <summary>
        /// Position where to start the drawing in X.
        /// </summary>
        protected int offsetX;

        /// <summary>
        /// Position where to start the drawing in Y.
        /// </summary>
        protected int offsetY;

        /// <summary>
        /// Array of BCGLabel.
        /// </summary>
        protected readonly List<BCGLabel> labels = new List<BCGLabel>();

        /// <summary>
        /// Push the label, left and top.
        /// </summary>
        protected int[] pushLabel = new int[] { 0, 0 };

        /// <summary>
        /// Constructor.
        /// </summary>
        protected BCGBarcode()
        {
            this.SetOffsetX(0);
            this.SetOffsetY(0);
            this.SetForegroundColor(new BCGColor(0x000000));
            this.SetBackgroundColor(new BCGColor(0xffffff));
            this.SetScale(1);
        }

        /// <summary>
        /// Parses the text before displaying it.
        /// </summary>
        /// <param name="text">The text.</param>
        public abstract void Parse(string text);

        /// <summary>
        /// Gets the foreground color of the barcode.
        /// </summary>
        /// <returns>The foreground color.</returns>
        public BCGColor GetForegroundColor()
        {
            return this.colorFg;
        }

        /// <summary>
        /// Sets the foreground color of the barcode.
        /// </summary>
        /// <param name="color">The foreground color.</param>
        public void SetForegroundColor(BCGColor color)
        {
            this.colorFg = color;
        }

        /// <summary>
        /// Gets the background color of the barcode.
        /// </summary>
        /// <returns>The background color.</returns>
        public BCGColor GetBackgroundColor()
        {
            return this.colorBg;
        }

        /// <summary>
        /// Sets the background color of the barcode.
        /// </summary>
        /// <param name="color">The background color.</param>
        public void SetBackgroundColor(BCGColor color)
        {
            this.colorBg = color;

            foreach (var label in this.labels)
            {
                label.SetBackgroundColor(this.colorBg);
            }
        }

        /// <summary>
        /// Sets the foreground and background color.
        /// </summary>
        /// <param name="foregroundColor">The foreground color.</param>
        /// <param name="backgroundColor">The background color.</param>
        public void SetColor(BCGColor foregroundColor, BCGColor backgroundColor)
        {
            this.SetForegroundColor(foregroundColor);
            this.SetBackgroundColor(backgroundColor);
        }

        /// <summary>
        /// Gets the scale of the barcode.
        /// </summary>
        /// <returns>The scale.</returns>
        public virtual int GetScale()
        {
            return this.scale;
        }

        /// <summary>
        /// Sets the scale of the barcode in pixel.
        /// If the scale is lower than 1, an exception is raised.
        /// </summary>
        /// <param name="scale">The scale.</param>
        public virtual void SetScale(int scale)
        {
            if (scale <= 0)
            {
                throw new BCGArgumentException("The scale must be larger than 0.", nameof(scale));
            }

            this.scale = scale;
        }

        /// <summary>
        /// Abstract method that draws the barcode on the surface.
        /// </summary>
        /// <param name="image">The surface.</param>
        public abstract void Draw(BCGSurface image);

        /// <summary>
        /// Returns the maximal size of a barcode.
        /// [0]->width
        /// [1]->height
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An array, [0] being the width, [1] being the height.</returns>
        public virtual int[] GetDimension(int width, int height)
        {
            var labels = this.GetBiggestLabels(false);
            var pixelsAround = new int[] { 0, 0, 0, 0 }; // TRBL
            if (labels.ContainsKey(BCGLabel.Position.Top))
            {
                var dimension = labels[BCGLabel.Position.Top].GetDimension();
                pixelsAround[0] += dimension[1];
            }

            if (labels.ContainsKey(BCGLabel.Position.Right))
            {
                var dimension = labels[BCGLabel.Position.Right].GetDimension();
                pixelsAround[1] += dimension[0];
            }

            if (labels.ContainsKey(BCGLabel.Position.Bottom))
            {
                var dimension = labels[BCGLabel.Position.Bottom].GetDimension();
                pixelsAround[2] += dimension[1];
            }

            if (labels.ContainsKey(BCGLabel.Position.Left))
            {
                var dimension = labels[BCGLabel.Position.Left].GetDimension();
                pixelsAround[3] += dimension[0];
            }

            var finalW = (width + this.offsetX) * this.scale;
            var finalH = (height + this.offsetY) * this.scale;

            // This section will check if a top/bottom label is too big for its width and left/right too big for its height
            var reversedLabels = this.GetBiggestLabels(true);
            foreach (var label in reversedLabels.Values)
            {
                var dimension = label.GetDimension();
                var alignment = label.GetAlignment();
                if (label.GetPosition() == BCGLabel.Position.Left || label.GetPosition() == BCGLabel.Position.Right)
                {
                    if (alignment == BCGLabel.Alignment.Top)
                    {
                        pixelsAround[2] = Math.Max(pixelsAround[2], dimension[1] - finalH);
                    }
                    else if (alignment == BCGLabel.Alignment.Center)
                    {
                        var temp = (int)Math.Ceiling((double)(dimension[1] - finalH) / 2);
                        pixelsAround[0] = Math.Max(pixelsAround[0], temp);
                        pixelsAround[2] = Math.Max(pixelsAround[2], temp);
                    }
                    else if (alignment == BCGLabel.Alignment.Bottom)
                    {
                        pixelsAround[0] = Math.Max(pixelsAround[0], dimension[1] - finalH);
                    }
                }
                else
                {
                    if (alignment == BCGLabel.Alignment.Left)
                    {
                        pixelsAround[1] = Math.Max(pixelsAround[1], dimension[0] - finalW);
                    }
                    else if (alignment == BCGLabel.Alignment.Center)
                    {
                        var temp = (int)Math.Ceiling((double)(dimension[0] - finalW) / 2);
                        pixelsAround[1] = Math.Max(pixelsAround[1], temp);
                        pixelsAround[3] = Math.Max(pixelsAround[3], temp);
                    }
                    else if (alignment == BCGLabel.Alignment.Right)
                    {
                        pixelsAround[3] = Math.Max(pixelsAround[3], dimension[0] - finalW);
                    }
                }
            }

            this.pushLabel[0] = pixelsAround[3];
            this.pushLabel[1] = pixelsAround[0];

            finalW = (width + this.offsetX) * this.scale + pixelsAround[1] + pixelsAround[3];
            finalH = (height + this.offsetY) * this.scale + pixelsAround[0] + pixelsAround[2];

            return new int[] { finalW, finalH };
        }

        /// <summary>
        /// Gets the X offset.
        /// </summary>
        /// <returns>The X offset.</returns>
        public int GetOffsetX()
        {
            return this.offsetX;
        }

        /// <summary>
        /// Sets the X offset.
        /// </summary>
        /// <param name="offsetX">The X offset.</param>
        public virtual void SetOffsetX(int offsetX)
        {
            if (this.offsetX < 0)
            {
                throw new BCGArgumentException("The offset X must be 0 or larger.", nameof(offsetX));
            }

            this.offsetX = offsetX;
        }

        /// <summary>
        /// Gets the Y offset.
        /// </summary>
        /// <returns>The Y offset.</returns>
        public int GetOffsetY()
        {
            return this.offsetY;
        }

        /// <summary>
        /// Sets the Y offset.
        /// </summary>
        /// <param name="offsetY">The Y offset.</param>
        public virtual void SetOffsetY(int offsetY)
        {
            if (this.offsetY < 0)
            {
                throw new BCGArgumentException("The offset Y must be 0 or larger.", nameof(offsetY));
            }

            this.offsetY = offsetY;
        }

        /// <summary>
        /// Adds the label to the drawing.
        /// </summary>
        /// <param name="label">The label.</param>
        public void AddLabel(BCGLabel label)
        {
            label.SetBackgroundColor(this.colorBg);
            this.labels.Add(label);
        }

        /// <summary>
        /// Removes the label from the drawing.
        /// </summary>
        /// <param name="label">The label.</param>
        public void RemoveLabel(BCGLabel label)
        {
            this.labels.Remove(label);
        }

        /// <summary>
        /// Clears the labels.
        /// </summary>
        public void ClearLabels()
        {
            this.labels.Clear();
        }


        /// <summary>
        /// Draws the text.
        /// The coordinate passed are the positions of the barcode.
        /// <paramref name="x1"/> and <paramref name="y1"/> represent the top left corner.
        /// <paramref name="x2"/> and <paramref name="y2"/> represent the bottom right corner.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x1">The top left corner X coordinate.</param>
        /// <param name="y1">The top left corner Y coordinate.</param>
        /// <param name="x2">The bottom right corner X coordinate.</param>
        /// <param name="y2">The bottom right corner Y coordinate.</param>
        protected virtual void DrawText(BCGSurface image, int x1, int y1, int x2, int y2)
        {
            foreach (var label in labels)
            {
                label.Draw(image,
                    (x1 + this.offsetX) * this.scale + this.pushLabel[0],
                    (y1 + this.offsetY) * this.scale + this.pushLabel[1],
                    (x2 + this.offsetX) * this.scale + this.pushLabel[0],
                    (y2 + this.offsetY) * this.scale + this.pushLabel[1]);
            }
        }

        /// <summary>
        /// Draws 1 pixel on the resource at a specific position with the foreground color.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        protected virtual void DrawPixel(BCGSurface image, int x, int y)
        {
            DrawPixel(image, x, y, BCGBarcode.COLOR_FG);
        }

        /// <summary>
        /// Draws 1 pixel on the resource at a specific position with a determined color.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="color">The color.</param>
        protected virtual void DrawPixel(BCGSurface image, int x, int y, int color)
        {
            var xR = (x + this.offsetX) * this.scale + this.pushLabel[0];
            var yR = (y + this.offsetY) * this.scale + this.pushLabel[1];

            // We always draw a rectangle
            image.FillRectangle(
                xR,
                yR,
                xR + this.scale - 1,
                yR + this.scale - 1,
                this.GetColor(image, color)
            );
        }

        /// <summary>
        /// Draws an empty rectangle on the resource at a specific position with the foreground color.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x1">The top left corner X coordinate.</param>
        /// <param name="y1">The top left corner Y coordinate.</param>
        /// <param name="x2">The bottom right corner X coordinate.</param>
        /// <param name="y2">The bottom right corner Y coordinate.</param>
        protected virtual void DrawRectangle(BCGSurface image, int x1, int y1, int x2, int y2)
        {
            this.DrawRectangle(image, x1, y1, x2, y2, BCGBarcode.COLOR_FG);
        }

        /// <summary>
        /// Draws an empty rectangle on the resource at a specific position with a determined color.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x1">The top left corner X coordinate.</param>
        /// <param name="y1">The top left corner Y coordinate.</param>
        /// <param name="x2">The bottom right corner X coordinate.</param>
        /// <param name="y2">The bottom right corner Y coordinate.</param>
        /// <param name="color">The color.</param>
        protected virtual void DrawRectangle(BCGSurface image, int x1, int y1, int x2, int y2, int color)
        {
            if (this.scale == 1)
            {
                image.FillRectangle(
                    (x1 + this.offsetX) + this.pushLabel[0],
                    (y1 + this.offsetY) + this.pushLabel[1],
                    (x2 + this.offsetX) + this.pushLabel[0],
                    (y2 + this.offsetY) + this.pushLabel[1],
                    this.GetColor(image, color)
                );
            }
            else
            {
                image.FillRectangle((x1 + this.offsetX) * this.scale + this.pushLabel[0], (y1 + this.offsetY) * this.scale + this.pushLabel[1], (x2 + this.offsetX) * this.scale + this.pushLabel[0] + this.scale - 1, (y1 + this.offsetY) * this.scale + this.pushLabel[1] + this.scale - 1, this.GetColor(image, color));
                image.FillRectangle((x1 + this.offsetX) * this.scale + this.pushLabel[0], (y1 + this.offsetY) * this.scale + this.pushLabel[1], (x1 + this.offsetX) * this.scale + this.pushLabel[0] + this.scale - 1, (y2 + this.offsetY) * this.scale + this.pushLabel[1] + this.scale - 1, this.GetColor(image, color));
                image.FillRectangle((x2 + this.offsetX) * this.scale + this.pushLabel[0], (y1 + this.offsetY) * this.scale + this.pushLabel[1], (x2 + this.offsetX) * this.scale + this.pushLabel[0] + this.scale - 1, (y2 + this.offsetY) * this.scale + this.pushLabel[1] + this.scale - 1, this.GetColor(image, color));
                image.FillRectangle((x1 + this.offsetX) * this.scale + this.pushLabel[0], (y2 + this.offsetY) * this.scale + this.pushLabel[1], (x2 + this.offsetX) * this.scale + this.pushLabel[0] + this.scale - 1, (y2 + this.offsetY) * this.scale + this.pushLabel[1] + this.scale - 1, this.GetColor(image, color));
            }
        }

        /// <summary>
        /// Draws a filled rectangle on the resource at a specific position with the foreground color.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x1">The top left corner X coordinate.</param>
        /// <param name="y1">The top left corner Y coordinate.</param>
        /// <param name="x2">The bottom right corner X coordinate.</param>
        /// <param name="y2">The bottom right corner Y coordinate.</param>
        protected virtual void DrawFilledRectangle(BCGSurface image, int x1, int y1, int x2, int y2)
        {
            this.DrawFilledRectangle(image, x1, y1, x2, y2, BCGBarcode.COLOR_FG);
        }

        /// <summary>
        /// Draws a filled rectangle on the resource at a specific position with a determined color.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x1">The top left corner X coordinate.</param>
        /// <param name="y1">The top left corner Y coordinate.</param>
        /// <param name="x2">The bottom right corner X coordinate.</param>
        /// <param name="y2">The bottom right corner Y coordinate.</param>
        /// <param name="color">The color.</param>
        protected virtual void DrawFilledRectangle(BCGSurface image, int x1, int y1, int x2, int y2, int color)
        {
            if (x1 > x2)
            { // Swap
                x1 ^= x2 ^= x1 ^= x2;
            }

            if (y1 > y2)
            { // Swap
                y1 ^= y2 ^= y1 ^= y2;
            }

            image.FillRectangle(
                (x1 + this.offsetX) * this.scale + this.pushLabel[0],
                (y1 + this.offsetY) * this.scale + this.pushLabel[1],
                (x2 + this.offsetX) * this.scale + this.pushLabel[0] + this.scale - 1,
                (y2 + this.offsetY) * this.scale + this.pushLabel[1] + this.scale - 1,
                this.GetColor(image, color)
            );
        }

        /// <summary>
        /// Allocates the color based on the integer.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="color">The color.</param>
        /// <returns>Implementation details of the color.</returns>
        protected SKColor GetColor(BCGSurface image, int color)
        {
            if (color == BCGBarcode.COLOR_BG)
            {
                return this.colorBg.Allocate(image);
            }
            else
            {
                return this.colorFg.Allocate(image);
            }
        }

        /// <summary>
        /// Returning the biggest label widths for LEFT/RIGHT and heights for TOP/BOTTOM.
        /// </summary>
        /// <param name="reversed">Indicates if the barcode has been rotated.</param>
        /// <returns>Position of the biggest barcode.</returns>
        private IDictionary<BCGLabel.Position, BCGLabel> GetBiggestLabels(bool reversed = false)
        {
            var searchLR = reversed ? 1 : 0;
            var searchTB = reversed ? 0 : 1;

            var labels = new Dictionary<BCGLabel.Position, BCGLabel>();
            foreach (var label in this.labels)
            {
                var position = label.GetPosition();
                if (labels.ContainsKey(position))
                {
                    var savedDimension = labels[position].GetDimension();
                    var dimension = label.GetDimension();
                    if (position == BCGLabel.Position.Left || position == BCGLabel.Position.Right)
                    {
                        if (dimension[searchLR] > savedDimension[searchLR])
                        {
                            labels[position] = label;
                        }
                    }
                    else
                    {
                        if (dimension[searchTB] > savedDimension[searchTB])
                        {
                            labels[position] = label;
                        }
                    }
                }
                else
                {
                    labels[position] = label;
                }
            }

            return labels;
        }
    }
}
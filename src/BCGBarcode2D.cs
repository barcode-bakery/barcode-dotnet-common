using BarcodeBakery.Common.Extensions;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// Holds all type of barcodes for 1D generation.
    /// </summary>
    public abstract class BCGBarcode2D : BCGBarcode
    {
        /// <summary>
        /// The X multipled by this scale.
        /// </summary>
        protected int scaleX;

        /// <summary>
        /// The Y multipled by this scale.
        /// </summary>
        protected int scaleY;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected BCGBarcode2D()
            : base()
        {
            this.SetScaleX(1);
            this.SetScaleY(1);
        }

        /// <summary>
        /// Returns the maximal size of a barcode.
        /// [0]->width
        /// [1]->height
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An array, [0] being the width, [1] being the height.</returns>
        public override int[] GetDimension(int width, int height)
        {
            return base.GetDimension(width * this.scaleX, height * this.scaleY);
        }

        /// <summary>
        /// Sets the scale of the barcode in pixel for X.
        /// If the scale is lower than 1, an exception is raised.
        /// </summary>
        /// <param name="scaleX">The scale for X.</param>
        protected virtual void SetScaleX(int scaleX)
        {
            if (scaleX <= 0)
            {
                throw new BCGArgumentException("The scale must be larger than 0.", nameof(scaleX));
            }

            this.scaleX = scaleX;
        }


        /// <summary>
        /// Sets the scale of the barcode in pixel for Y.
        /// If the scale is lower than 1, an exception is raised.
        /// </summary>
        /// <param name="scaleY">The scale for Y.</param>
        protected virtual void SetScaleY(int scaleY)
        {
            if (scaleY <= 0)
            {
                throw new BCGArgumentException("The scale must be larger than 0.", nameof(scaleY));
            }

            this.scaleY = scaleY;
        }

        /// <summary>
        /// Draws the text.
        /// The coordinate passed are the positions of the barcode.
        /// <paramref name="x1"/> and <paramref name="y1"/> represent the top left corner.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x1">X1.</param>
        /// <param name="y1">Y1.</param>
        /// <param name="x2">X2.</param>
        /// <param name="y2">Y2.</param>
        protected override void DrawText(BCGSurface image, int x1, int y1, int x2, int y2)
        {
            foreach (var label in labels)
            {
                label.Draw(image,
                    (x1 + this.offsetX) * this.scale * this.scaleX + this.pushLabel[0],
                    (y1 + this.offsetY) * this.scale * this.scaleY + this.pushLabel[1],
                    (x2 + this.offsetX) * this.scale * this.scaleX + this.pushLabel[0],
                    (y2 + this.offsetY) * this.scale * this.scaleY + this.pushLabel[1]);
            }
        }

        /// <summary>
        /// Draws 1 pixel on the resource at a specific position with a determined color.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="color">The color.</param>
        protected override void DrawPixel(BCGSurface image, int x, int y, int color)
        {
            int scaleX = this.scale * this.scaleX;
            int scaleY = this.scale * this.scaleY;

            int xR = (x + this.offsetX) * scaleX + this.pushLabel[0];
            int yR = (y + this.offsetY) * scaleY + this.pushLabel[1];

            image.FillRectangle(
                xR,
                yR,
                xR + scaleX - 1,
                yR + scaleY - 1,
                this.GetColor(image, color)
            );
        }

        /// <summary>
        /// Draws an empty rectangle on the resource at a specific position with a determined color.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x1">X1.</param>
        /// <param name="y1">Y1.</param>
        /// <param name="x2">X2.</param>
        /// <param name="y2">Y2.</param>
        /// <param name="color">The color.</param>
        protected override void DrawRectangle(BCGSurface image, int x1, int y1, int x2, int y2, int color)
        {
            int scaleX = this.scale * this.scaleX;
            int scaleY = this.scale * this.scaleY;

            if (this.scale == 1)
            {
                image.FillRectangle(
                    (x1 + this.offsetX) * scaleX + this.pushLabel[0],
                    (y1 + this.offsetY) * scaleY + this.pushLabel[1],
                    (x2 + this.offsetX) * scaleX + this.pushLabel[0],
                    (y2 + this.offsetY) * scaleY + this.pushLabel[1],
                    this.GetColor(image, color)
                );
            }
            else
            {
                image.FillRectangle((x1 + this.offsetX) * scaleX + this.pushLabel[0], (y1 + this.offsetY) * scaleY + this.pushLabel[1], (x2 + this.offsetX) * scaleX + scaleX - 1 + this.pushLabel[0], (y1 + this.offsetY) * scaleY + scaleY - 1 + this.pushLabel[1], this.GetColor(image, color));
                image.FillRectangle((x1 + this.offsetX) * scaleX + this.pushLabel[0], (y1 + this.offsetY) * scaleY + this.pushLabel[1], (x1 + this.offsetX) * scaleX + scaleX - 1 + this.pushLabel[0], (y2 + this.offsetY) * scaleY + scaleY - 1 + this.pushLabel[1], this.GetColor(image, color));
                image.FillRectangle((x2 + this.offsetX) * scaleX + this.pushLabel[0], (y1 + this.offsetY) * scaleY + this.pushLabel[1], (x2 + this.offsetX) * scaleX + scaleX - 1 + this.pushLabel[0], (y2 + this.offsetY) * scaleY + scaleY - 1 + this.pushLabel[1], this.GetColor(image, color));
                image.FillRectangle((x1 + this.offsetX) * scaleX + this.pushLabel[0], (y2 + this.offsetY) * scaleY + this.pushLabel[1], (x2 + this.offsetX) * scaleX + scaleX - 1 + this.pushLabel[0], (y2 + this.offsetY) * scaleY + scaleY - 1 + this.pushLabel[1], this.GetColor(image, color));
            }
        }

        /// <summary>
        /// Draws a filled rectangle on the resource at a specific position with a determined color.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="x1">X1.</param>
        /// <param name="y1">Y1.</param>
        /// <param name="x2">X2.</param>
        /// <param name="y2">Y2.</param>
        /// <param name="color">The color.</param>
        protected override void DrawFilledRectangle(BCGSurface image, int x1, int y1, int x2, int y2, int color)
        {
            int scaleX = this.scale * this.scaleX;
            int scaleY = this.scale * this.scaleY;

            if (x1 > x2) // Swap
            {
                // XOR Swap not available in C#
                int t = x1;
                x1 = x2;
                x2 = t;
            }

            if (y1 > y2) // Swap
            {
                // XOR Swap not available in C#
                int t = y1;
                y1 = y2;
                y2 = t;
            }


            image.FillRectangle(
                (x1 + this.offsetX) * scaleX + this.pushLabel[0],
                (y1 + this.offsetY) * scaleY + this.pushLabel[1],
                (x2 + this.offsetX) * scaleX + scaleX - 1 + this.pushLabel[0],
                (y2 + this.offsetY) * scaleY + scaleY - 1 + this.pushLabel[1],
                this.GetColor(image, color)
            );
        }
    }
}

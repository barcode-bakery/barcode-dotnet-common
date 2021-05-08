using BarcodeBakery.Common.Drawer;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// Holds the drawing resource.
    /// You can use <see cref="GetImage"/> to add other kind of form not held into these classes.
    /// </summary>
    public class BCGDrawing : IDisposable
    {
        /// <summary>
        /// The image format.
        /// </summary>
        public enum ImageFormat
        {
            /// <summary>
            /// The ASTC image format.
            /// </summary>
            Astc,

            /// <summary>
            /// The BMP image format.
            /// </summary>
            Bmp,

            /// <summary>
            /// The Adobe DNG image format.
            /// </summary>
            Dng,

            /// <summary>
            /// The GIF image format.
            /// </summary>
            Gif,

            /// <summary>
            /// The HEIF or High Efficiency Image File format.
            /// </summary>
            Heif,

            /// <summary>
            /// The ICO image format.
            /// </summary>
            Ico,

            /// <summary>
            /// The JPEG image format.
            /// </summary>
            Jpeg,

            /// <summary>
            /// The KTX image format.
            /// </summary>
            Ktx,

            /// <summary>
            /// The PKM image format.
            /// </summary>
            Pkm,

            /// <summary>
            /// The PNG image format.
            /// </summary>
            Png,

            /// <summary>
            /// The WBMP image format.
            /// </summary>
            Wbmp,

            /// <summary>
            /// The WEBP image format.
            /// </summary>
            Webp
        }

        private readonly IReadOnlyDictionary<ImageFormat, SKEncodedImageFormat> encodingMap = new Dictionary<ImageFormat, SKEncodedImageFormat>
        {
            { ImageFormat.Astc, SKEncodedImageFormat.Astc },
            { ImageFormat.Bmp, SKEncodedImageFormat.Bmp },
            { ImageFormat.Dng, SKEncodedImageFormat.Dng },
            { ImageFormat.Gif, SKEncodedImageFormat.Gif },
            { ImageFormat.Heif, SKEncodedImageFormat.Heif },
            { ImageFormat.Ico, SKEncodedImageFormat.Ico },
            { ImageFormat.Jpeg, SKEncodedImageFormat.Jpeg },
            { ImageFormat.Ktx, SKEncodedImageFormat.Ktx },
            { ImageFormat.Pkm, SKEncodedImageFormat.Pkm },
            { ImageFormat.Png, SKEncodedImageFormat.Png },
            { ImageFormat.Wbmp, SKEncodedImageFormat.Wbmp },
            { ImageFormat.Webp, SKEncodedImageFormat.Webp }
        };

        #region MEMBERS
        private int w, h;
        private readonly BCGColor color;
        private BCGSurface? im;
        private BCGBarcode? barcode;
        private float? dpi;
        private float rotateDegree;
        #endregion

        private Exception? exceptionToDraw;

        /// <summary>
        /// Creates a drawing surface by indicating its background color.
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <param name="color">Background color.</param>
        public BCGDrawing(BCGBarcode? barcode, BCGColor? color = null)
        {
            this.barcode = barcode;
            this.im = null;
            this.color = color ?? new BCGColor(255, 255, 255);
            this.dpi = 0;
            this.rotateDegree = 0.0f;
        }

        /// <summary>
        /// Gets the image resource.
        /// </summary>
        /// <returns>The surface abstraction where the barcode is drawn.</returns>
        public BCGSurface? GetImage()
        {
            return this.im;
        }

        /// <summary>
        /// Sets the image resource.
        /// </summary>
        /// <param name="image">The surface.</param>
        public void SetImage(ref BCGSurface image)
        {
            this.im = image;
        }

        /// <summary>
        /// Gets barcode for drawing.
        /// </summary>
        /// <returns>The barcode.</returns>
        public BCGBarcode? GetBarcode()
        {
            return this.barcode;
        }

        /// <summary>
        /// Set barcode for drawing.
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        public void SetBarcode(BCGBarcode barcode)
        {
            this.barcode = barcode;
        }

        /// <summary>
        /// Gets the DPI for supported filetype.
        /// </summary>
        /// <returns>The DPI.</returns>
        public float? GetDPI()
        {
            return this.dpi;
        }

        /// <summary>
        /// Sets the DPI for supported filetype.
        /// </summary>
        /// <param name="dpi">The DPI.</param>
        public void SetDPI(float? dpi)
        {
            this.dpi = dpi;
        }

        /// <summary>
        /// Gets the rotation angle in degree. The rotation is clockwise.
        /// </summary>
        /// <returns>Rotation angle in degree.</returns>
        public float GetRotationAngle()
        {
            return this.rotateDegree;
        }

        /// <summary>
        /// Sets the rotation angle in degree. The rotation is clockwise.
        /// </summary>
        /// <param name="degree">Rotation angle in degree.</param>
        public void SetRotationAngle(float degree)
        {
            this.rotateDegree = degree;
        }

        /// <summary>
        /// Draws the barcode on the surface.
        /// </summary>
        private void Draw()
        {
            if (this.exceptionToDraw != null || this.barcode == null)
            {
                var paint = new SKPaint
                {
                    Typeface = null, // Picks the default
                    TextSize = 14,
                    IsAntialias = true
                };

                var message = exceptionToDraw?.Message ?? "No barcode available";
                SKRect textSize = SKRect.Empty;
                paint.MeasureText(message, ref textSize);

                var finalWidth = textSize.Width + textSize.Left;
                var finalHeight = textSize.Height;
                this.im = new BCGSurface(SKSurface.Create(new SKImageInfo((int)Math.Ceiling(finalWidth), (int)Math.Ceiling(finalHeight))));
                this.im.SKSurface.Canvas.Clear(this.color.Allocate(this.im));

                paint.Color = new BCGColor("black").Allocate(this.im);

                var drawPositionX = 0;
                var drawPositionY = 0;
                this.im.SKSurface.Canvas.Save();
                this.im.SKSurface.Canvas.Translate(drawPositionX, drawPositionY - textSize.Top);
                this.im.SKSurface.Canvas.DrawText(message, drawPositionX, drawPositionY, paint);
                this.im.SKSurface.Canvas.Restore();
            }
            else
            {
                var size = this.barcode.GetDimension(0, 0);
                this.w = Math.Max(1, size[0]);
                this.h = Math.Max(1, size[1]);
                this.Init();
                this.barcode.Draw(this.im!); // !Has been init
            }
        }

        /// <summary>
        /// Saves the image into a stream.
        /// Please note that the stream is not rewinded.
        /// </summary>
        /// <param name="imageStyle">The Image Style.</param>
        /// <param name="memoryStream">The Memory Stream.</param>
        public async Task FinishAsync(ImageFormat imageStyle, MemoryStream memoryStream)
        {
            Draw();
            await SaveBitmapInternalAsync(async (MemoryStream stream) =>
            {
                await stream.CopyToAsync(memoryStream);
            }, imageStyle);
        }


        /// <summary>
        /// Saves the image into a file.
        /// </summary>
        /// <param name="imageStyle">The Image Style.</param>
        /// <param name="path">The file path.</param>
        public async Task FinishAsync(ImageFormat imageStyle, string path)
        {
            Draw();
            if (!string.IsNullOrEmpty(path))
            {
                await SaveBitmapInternalAsync(async (MemoryStream stream) =>
                {
                    using FileStream file = new FileStream(path, FileMode.Create, System.IO.FileAccess.Write);
                    byte[] bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, (int)stream.Length);
                    await file.WriteAsync(bytes, 0, bytes.Length);
                }, imageStyle);
            }
        }

        private async Task SaveBitmapInternalAsync(Func<MemoryStream, Task> methodAsync, ImageFormat imageFormat)
        {
            if (encodingMap.TryGetValue(imageFormat, out var imageStyle))
            {
                SKSurface? finalBitmap = null;
                try
                {
                    using var bitmap = this.GetImage()!.SKSurface;
                    finalBitmap = bitmap;
                    if (this.rotateDegree != 0.0)
                    {
                        finalBitmap = this.RotateImage(bitmap, this.rotateDegree);
                    }

                    if (this.dpi.HasValue && this.dpi.Value != 0.0f)
                    {
                        // TODO
                        //finalBitmap.SetResolution(this.dpi.Value, this.dpi.Value);
                    }
                    using var stream = new MemoryStream();
                    MemoryStream finalStream = stream;
                    finalBitmap.Snapshot().Encode(imageStyle, 100).SaveTo(finalStream);

                    BCGDraw? drawer = null;
                    try
                    {
                        // if (imageStyle.Guid == ImageFormat.Png.Guid)
                        //  {
                        drawer = new BCGDrawPNG();
                        finalStream = drawer.Draw(finalStream);
                        //  }
                        //else if (imageStyle.Guid == ImageFormat.Jpeg.Guid)
                        // {
                        //     drawer = new BCGDrawJPG();
                        //     finalStream = drawer.Draw(finalStream);
                        // }

                        finalStream.Seek(0, SeekOrigin.Begin);
                        await methodAsync(finalStream);
                    }
                    finally
                    {
                        if (drawer != null)
                        {
                            drawer.Dispose();
                        }
                    }
                }
                finally
                {
                    if (finalBitmap != null)
                    {
                        finalBitmap.Dispose();
                    }
                }
            }
            else
            {
                throw new Exception("The image style does not exist.");
            }
        }

        /**
         * Writes the Error on the picture.
         *
         * @param Exception $exception
         */
        public void DrawException(Exception exception)
        {
            this.exceptionToDraw = exception;
        }

        private SKSurface RotateImage(SKSurface surface, float angle)
        {
            if (surface.Canvas.GetDeviceClipBounds(out var deviceClipBounds))
            {
                var radians = Math.PI * angle / 180;
                var sine = (float)Math.Abs(Math.Sin(radians));
                var cosine = (float)Math.Abs(Math.Cos(radians));
                var originalWidth = deviceClipBounds.Width;
                var originalHeight = deviceClipBounds.Height;

                var rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
                var rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

                var newSurface = SKSurface.Create(new SKImageInfo(rotatedWidth, rotatedHeight));
                newSurface.Canvas.Clear();
                newSurface.Canvas.Translate(rotatedWidth / 2, rotatedHeight / 2);
                newSurface.Canvas.RotateDegrees(angle);
                newSurface.Canvas.Translate(-originalWidth / 2, -originalHeight / 2);
                newSurface.Canvas.DrawSurface(surface, 0, 0);

                return newSurface;
            }

            return surface;
        }

        /**
         * Init Image and color background
         */
        private void Init()
        {
            // We clear the drawing surface.
            this.im = new BCGSurface(SKSurface.Create(new SKImageInfo(this.w, this.h)));
            this.im.SKSurface.Canvas.Clear(this.color.Allocate(this.im));
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the drawing.
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
        /// Disposes the drawing.
        /// </summary>
        /// <param name="disposing">Is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.im != null)
                {
                    //this.im.Dispose();
                    this.im = null;
                }
            }
        }

        #endregion
    }
}

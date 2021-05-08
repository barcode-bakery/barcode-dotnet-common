using System;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// Holds the label.
    /// </summary>
    public class BCGLabel
    {
        /// <summary>
        /// The position.
        /// </summary>
        public enum Position
        {
            /// <summary>
            /// Top.
            /// </summary>
            Top = 0,

            /// <summary>
            /// Right.
            /// </summary>
            Right = 1,

            /// <summary>
            /// Bottom.
            /// </summary>
            Bottom = 2,

            /// <summary>
            /// Left.
            /// </summary>
            Left = 3
        }

        /// <summary>
        /// The alignement.
        /// </summary>
        public enum Alignment
        {
            /// <summary>
            /// Left, when used horizontally.
            /// </summary>
            Left,

            /// <summary>
            /// Top, when used vertically.
            /// </summary>
            Top,

            /// <summary>
            /// Center.
            /// </summary>
            Center,

            /// <summary>
            /// Right, when used horizontally.
            /// </summary>
            Right,

            /// <summary>
            /// Bottom, when used vertically.
            /// </summary>
            Bottom
        }

        private BCGFont font = null!; // !Assigned in the constructor.
        private string text = null!; // !Assigned in the constructor.
        private Position position;
        private Alignment alignment;
        private int offset;
        private int spacing;
        private int rotationAngle;
        private BCGColor backgroundColor = null!; // !Assigned in the constructor.
        private BCGColor foregroundColor = null!; // !Assigned in the constructor.

        /**
         * Constructor.
         *
         * @param string $text
         * @param BCGFont $font
         * @param int $position
         * @param int $alignment
         */
        public BCGLabel(string text = "", BCGFont? font = null, Position position = Position.Bottom, Alignment alignment = Alignment.Center)
        {
            font ??= new BCGFont("Arial", 12);
            this.SetFont(font);
            this.SetText(text);
            this.SetPosition(position);
            this.SetAlignment(alignment);
            this.SetSpacing(4);
            this.SetOffset(0);
            this.SetRotationAngle(0);
            this.SetBackgroundColor(new BCGColor("white"));
            this.SetForegroundColor(new BCGColor("black"));
        }

        /**
         * Gets the text.
         *
         * @return string
         */
        public string GetText()
        {
            return this.font.GetText();
        }

        /**
         * Sets the text.
         *
         * @param string $text
         */
        public void SetText(string text)
        {
            this.text = text;
            this.font.SetText(this.text);
        }

        /**
         * Gets the font.
         *
         * @return BCGFont
         */
        public BCGFont GetFont()
        {
            return this.font;
        }

        /**
         * Sets the font.
         *
         * @param BCGFont $font
         */
        public void SetFont(BCGFont font)
        {
            if (font == null)
            {
                throw new BCGArgumentException("Font cannot be null.", nameof(font));
            }

            this.font = (BCGFont)font.Clone();
            this.font.SetText(this.text);
            this.font.SetRotationAngle(this.rotationAngle);
            this.font.SetBackgroundColor(this.backgroundColor);
            this.font.SetForegroundColor(this.foregroundColor);
        }

        /**
         * Gets the text position for drawing.
         *
         * @return int
         */
        public Position GetPosition()
        {
            return this.position;
        }

        /// <summary>
        /// .NET Only.
        /// Sets the text position for drawing.
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Position position)
        {
            this.position = position;
        }

        /**
         * Gets the text alignment for drawing.
         *
         * @return int
         */
        public Alignment GetAlignment()
        {
            return this.alignment;
        }

        /// <summary>
        /// .NET Only.
        /// Sets the text alignment for drawing.
        /// </summary>
        /// <param name="alignment"></param>
        public void SetAlignment(Alignment alignment)
        {
            this.alignment = alignment;
        }

        /**
         * Gets the offset.
         *
         * @return int
         */
        public int GetOffset()
        {
            return this.offset;
        }

        /**
         * Sets the offset.
         *
         * @param int $offset
         */
        public void SetOffset(int offset)
        {
            this.offset = offset;
        }

        /**
         * Gets the spacing.
         *
         * @return int
         */
        public int GetSpacing()
        {
            return this.spacing;
        }

        /**
         * Sets the spacing.
         *
         * @param int $spacing
         */
        public void SetSpacing(int spacing)
        {
            this.spacing = Math.Max(0, spacing);
        }

        /**
         * Gets the rotation angle in degree.
         *
         * @return int
         */
        public int GetRotationAngle()
        {
            return this.font.GetRotationAngle();
        }

        /**
         * Sets the rotation angle in degree.
         *
         * @param int $rotationAngle
         */
        public void SetRotationAngle(int rotationAngle)
        {
            this.rotationAngle = rotationAngle;
            this.font.SetRotationAngle(this.rotationAngle);
        }

        /**
         * Gets the background color in case of rotation.
         *
         * @return BCGColor
         */
        public BCGColor GetBackgroundColor()
        {
            return this.backgroundColor;
        }

        /**
         * Sets the background color in case of rotation.
         *
         * @param BCGColor $backgroundColor
         */
        public /*internal*/ void SetBackgroundColor(BCGColor backgroundColor)
        {
            this.backgroundColor = backgroundColor;
            this.font.SetBackgroundColor(this.backgroundColor);
        }

        /**
         * Gets the foreground color.
         *
         * @return BCGColor
         */
        public BCGColor GetForegrounrdColor()
        {
            return this.font.GetForegroundColor();
        }

        /**
         * Sets the foreground color.
         *
         * @param BCGColor $foregroundColor
         */
        public void SetForegroundColor(BCGColor foregroundColor)
        {
            this.foregroundColor = foregroundColor;
            this.font.SetForegroundColor(this.foregroundColor);
        }

        /**
         * Gets the dimension taken by the label, including the spacing and offset.
         * [0]: width
         * [1]: height
         *
         * @return int[]
         */
        public int[] GetDimension()
        {
            var dimension = this.font.GetDimension();
            int w = dimension[0];
            int h = dimension[1];
            if (this.position == Position.Top || this.position == Position.Bottom)
            {
                h += this.spacing;
                w += Math.Max(0, this.offset);
            }
            else
            {
                w += this.spacing;
                h += Math.Max(0, this.offset);
            }

            return new int[] { w, h };
        }

        /**
         * Draws the text.
         * The coordinate passed are the positions of the barcode.
         * $x1 and $y1 represent the top left corner.
         * $x2 and $y2 represent the bottom right corner.
         *
         * @param resource $im
         * @param int $x1
         * @param int $y1
         * @param int $x2
         * @param int $y2
         */
        public /*internal*/ void Draw(BCGSurface image, int x1, int y1, int x2, int y2)
        {
            var x = 0;
            var y = 0;

            var fontDimension = this.font.GetDimension();

            if (this.position == Position.Top || this.position == Position.Bottom)
            {
                if (this.position == Position.Top)
                {
                    y = y1 - this.spacing - fontDimension[1];
                }
                else if (this.position == Position.Bottom)
                {
                    y = y2 + this.spacing;
                }

                if (this.alignment == Alignment.Center)
                {
                    x = (x2 - x1) / 2 + x1 - fontDimension[0] / 2 + this.offset;
                }
                else if (this.alignment == Alignment.Left)
                {
                    x = x1 + this.offset;
                }
                else
                {
                    x = x2 + this.offset - fontDimension[0];
                }
            }
            else
            {
                if (this.position == Position.Left)
                {
                    x = x1 - this.spacing - fontDimension[0];
                }
                else if (this.position == Position.Right)
                {
                    x = x2 + this.spacing;
                }

                if (this.alignment == Alignment.Center)
                {
                    y = (y2 - y1) / 2 + y1 - fontDimension[1] / 2 + this.offset;
                }
                else if (this.alignment == Alignment.Top)
                {
                    y = y1 + this.offset;
                }
                else
                {
                    y = y2 + this.offset - fontDimension[1];
                }
            }

            this.font.SetText(this.text);
            this.font.Draw(image, x, y);
        }
    }
}
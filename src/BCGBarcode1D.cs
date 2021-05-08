namespace BarcodeBakery.Common
{
    /// <summary>
    /// Holds all type of barcodes for 1D generation.
    /// </summary>
    public abstract class BCGBarcode1D : BCGBarcode
    {
        /// <summary>
        /// The value used in the label to mark the usage of the default label.
        /// </summary>
        public const string AUTO_LABEL = "##!!AUTO_LABEL!!##";

        /// <summary>
        /// Thickness of the barcode (usually the height).
        /// </summary>
        protected int thickness;

        /// <summary>
        /// Characters that can be displayed in the barcode.
        /// </summary>
        protected string[] keys = null!; // !Assigned by the subclasses.

        /// <summary>
        /// Code corresponding to the characters.
        /// </summary>
        protected string[] code = null!; // !Assigned by the subclasses.

        /// <summary>
        /// X Position where we are supposed to draw.
        /// </summary>
        protected int positionX;

        /// <summary>
        /// The font.
        /// </summary>
        protected BCGFont font = null!; // !Assigned in the constructor.

        /// <summary>
        /// The text to parse.
        /// </summary>
        protected string text;

        /// <summary>
        /// The checksum value, if supported.
        /// </summary>
        protected int[]? checksumValue;

        /// <summary>
        /// Indicates if the checksum is displayed.
        /// </summary>
        protected bool displayChecksum;

        /// <summary>
        /// Simple label for the barcode.
        /// </summary>
        protected string? label;

        /// <summary>
        /// Default label for the barcode.
        /// </summary>
        protected BCGLabel defaultLabel;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected BCGBarcode1D()
            : base()
        {
            this.SetThickness(30);

            this.defaultLabel = new BCGLabel();
            this.defaultLabel.SetPosition(BCGLabel.Position.Bottom);
            this.SetLabel(AUTO_LABEL);
            
            this.SetFont(new BCGFont("Arial", 8));

            this.text = "";
            this.checksumValue = null;
            this.positionX = 0;
        }

        /// <summary>
        /// Gets the thickness.
        /// </summary>
        /// <returns>The thickness.</returns>
        public int GetThickness()
        {
            return this.thickness;
        }

        /// <summary>
        /// Sets the thickness.
        /// </summary>
        /// <param name="thickness">The thickness.</param>
        public void SetThickness(int thickness)
        {
            if (thickness <= 0)
            {
                throw new BCGArgumentException("The thickness must be larger than 0.", nameof(thickness));
            }

            this.thickness = thickness;
        }

        /// <summary>
        /// Gets the label.
        /// If the label was set to <see cref="AUTO_LABEL"/>, the label will display the value from the text parsed.
        /// </summary>
        /// <returns>The label string.</returns>
        public virtual string? GetLabel()
        {
            var label = this.label;
            if (this.label == BCGBarcode1D.AUTO_LABEL)
            {
                string? checksum;
                label = this.text;
                if (this.displayChecksum == true && (checksum = this.ProcessChecksum()) != null)
                {
                    label += checksum;
                }
            }

            return label;
        }

        /// <summary>
        /// Sets the label.
        /// You can use <see cref="AUTO_LABEL"/> to have the label automatically written based on the parsed text.
        /// </summary>
        /// <param name="label">The label or <see cref="AUTO_LABEL"/>.</param>
        public virtual void SetLabel(string? label)
        {
            this.label = label;
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <returns>The font.</returns>
        public BCGFont GetFont()
        {
            return this.font;
        }

        /// <summary>
        /// Saves the font.
        /// </summary>
        /// <param name="font">The font.</param>
        public virtual void SetFont(BCGFont font)
        {
            this.font = font;
        }

        /// <summary>
        /// Parses the text before displaying it.
        /// </summary>
        /// <param name="text">The text.</param>
        public override void Parse(string text)
        {
            this.text = text;
            this.checksumValue = null; // Reset checksumValue
            this.Validate();

            this.AddDefaultLabel();
        }

        /// <summary>
        /// Gets the checksum of a Barcode.
        /// If no checksum is available, return null.
        /// </summary>
        /// <returns>The checksum or null.</returns>
        public string? GetChecksum()
        {
            return this.ProcessChecksum();
        }

        /// <summary>
        /// Sets if the checksum is displayed with the label or not.
        /// The checksum must be activated in some case to make this variable effective.
        /// </summary>
        /// <param name="display">Toggle to display the checksum on the label.</param>
        public void SetDisplayChecksum(bool display)
        {
            this.displayChecksum = display;
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
            return base.GetDimension(width, height);
        }

        /// <summary>
        /// Returns the maximal size of a barcode.
        /// This method exists because some sub-class need to know the base size of the barcode.
        /// [0]->width
        /// [1]->height
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An array, [0] being the width, [1] being the height.</returns>
        public virtual int[] Get1DDimension(int width, int height)
        {
            return base.GetDimension(width, height);
        }

        /// <summary>
        /// Adds the default label.
        /// </summary>
        protected virtual void AddDefaultLabel()
        {
            var label = this.GetLabel();
            var font = this.font;
            if (label != null && label != "" && font != null && this.defaultLabel != null)
            {
                this.defaultLabel.SetText(label);
                this.defaultLabel.SetFont(font);
                this.AddLabel(this.defaultLabel);
            }
        }

        /// <summary>
        /// Validates the input.
        /// </summary>
        protected virtual void Validate()
        {
            // No validation in the abstract class.
        }

        /// <summary>
        /// Returns the index in <see cref="keys"/> (useful for checksum).
        /// </summary>
        /// <param name="cvar">The character.</param>
        /// <returns>The position.</returns>
        protected int FindIndex(char cvar)
        {
            return FindIndex(cvar.ToString());
        }

        /// <summary>
        /// Returns the index in <see cref="keys"/> (useful for checksum).
        /// </summary>
        /// <param name="var">The character.</param>
        /// <returns>The position.</returns>
        protected int FindIndex(string var)
        {
            return ArraySearch(var, this.keys);
        }

        /// <summary>
        /// Makes an array search.
        /// </summary>
        /// <param name="cSearch">The character.</param>
        /// <param name="strArray">The array.</param>
        /// <returns>The position.</returns>
        protected static int ArraySearch(char cSearch, string[] strArray)
        {
            return ArraySearch(cSearch.ToString(), strArray);
        }

        /// <summary>
        /// Makes an array search.
        /// </summary>
        /// <param name="strSearch">The character.</param>
        /// <param name="strArray">The array.</param>
        /// <returns>The position.</returns>
        protected static int ArraySearch(string strSearch, string[] strArray)
        {
            int c = strArray.Length;
            for (int i = 0; i < c; i++)
            {
                if (strArray[i].Equals(strSearch))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the code of the char (useful for drawing bars).
        /// </summary>
        /// <param name="cvar">The character.</param>
        /// <returns>The code.</returns>
        protected string? FindCode(char cvar)
        {
            return FindCode(cvar.ToString());
        }

        /// <summary>
        /// Returns the code of the char (useful for drawing bars).
        /// </summary>
        /// <param name="var">The character.</param>
        /// <returns>The code.</returns>
        protected string? FindCode(string var)
        {
            int nIndex = this.FindIndex(var);
            if (nIndex >= 0)
            {
                return this.code[nIndex];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Draws all chars thanks to <paramref name="code"/>. This convenient method always begin with a space.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="code">The code.</param>
        protected virtual void DrawChar(BCGSurface image, string code)
        {
            this.DrawChar(image, code, true);
        }

        /// <summary>
        /// Draws all chars thanks to <paramref name="code"/>. If <paramref name="startBar"/> is true, the line begins by a space.
        /// If <paramref name="startBar"/> is false, the line begins by a bar.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="code">The code.</param>
        /// <param name="startBar">True if we begin with a space.</param>
        protected virtual void DrawChar(BCGSurface image, string code, bool startBar)
        {
            var colors = new int[2] { BCGBarcode1D.COLOR_FG, BCGBarcode1D.COLOR_BG };
            var currentColor = startBar ? 0 : 1;
            for (var i = 0; i < code.Length; i++)
            {
                if (int.TryParse(code[i].ToString(), out var nValue))
                {
                    for (var j = 0; j < nValue + 1; j++)
                    {
                        this.DrawSingleBar(image, colors[currentColor]);
                        this.NextX();
                    }
                }

                currentColor = (currentColor + 1) % 2;
            }
        }

        /// <summary>
        /// Draws a Bar of <paramref name="color"/> depending of the resolution.
        /// </summary>
        /// <param name="image">The surface.</param>
        /// <param name="color">The color.</param>
        protected void DrawSingleBar(BCGSurface image, int color)
        {
            this.DrawFilledRectangle(image, this.positionX, 0, this.positionX, this.thickness - 1, color);
        }

        /// <summary>
        /// Moving the pointer right to write a bar.
        /// </summary>
        protected void NextX()
        {
            this.positionX++;
        }

        /// <summary>
        /// Method that saves NULL into the checksumValue. This means no checksum
        /// but this method should be overloaded when needed.
        /// </summary>
        protected virtual void CalculateChecksum()
        {
            this.checksumValue = null;
        }

        /// <summary>
        /// Returns NULL because there is no checksum. This method should be
        /// overloaded to return correctly the checksum in string with checksumValue.
        /// </summary>
        /// <returns>The checksum value.</returns>
        protected virtual string? ProcessChecksum()
        {
            return null;
        }
    }
}
using System;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// Parse Exception.
    /// </summary>
    public class BCGParseException : Exception
    {
        /// <summary>
        /// The barcode code we are using.
        /// </summary>
        protected string? barcode;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BCGParseException()
        {
        }

        /// <summary>
        /// Constructor with error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public BCGParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor with error message and inner exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BCGParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor with specific message for a parameter.
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <param name="message">The message.</param>
        public BCGParseException(string barcode, string message)
            : base(message)
        {
            this.barcode = barcode;
        }
    }
}

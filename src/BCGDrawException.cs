using System;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// Draw Exception.
    /// </summary>
    public class BCGDrawException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BCGDrawException()
        {
        }

        /// <summary>
        /// Constructor with error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public BCGDrawException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor with error message and inner exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BCGDrawException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

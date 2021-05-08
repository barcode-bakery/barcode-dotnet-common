using System;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// Argument Exception.
    /// </summary>
    public class BCGArgumentException : ArgumentException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BCGArgumentException()
        {
        }

        /// <summary>
        /// Constructor with error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public BCGArgumentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor with error message and inner exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BCGArgumentException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor with specific message for a parameter.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="paramName">The params.</param>
        public BCGArgumentException(string message, string paramName)
            : base(message, paramName)
        {
        }

        /// <summary>
        /// Constructor with specific message for a parameter and inner exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="paramName">The params.</param>
        /// <param name="innerException">The inner exception.</param>
        public BCGArgumentException(string message, string paramName, Exception innerException)
            : base(message, paramName,  innerException)
        {
        }
    }
}

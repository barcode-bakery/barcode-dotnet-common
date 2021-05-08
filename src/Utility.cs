using System;

namespace BarcodeBakery.Common
{
    /// <summary>
    /// Utility class for the barcodes.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Executes a substring functionality without throwing an exception if out of bound.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>The new string.</returns>
        public static string SafeSubstring(string? str, int startIndex)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return Utility.SafeSubstring(str, startIndex, str.Length - startIndex);
        }

        /// <summary>
        /// Executes a substring functionality without throwing an exception if out of bound.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>The new string.</returns>
        public static string SafeSubstring(string? str, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            if (startIndex < 0)
            {
                startIndex = Math.Max(0, str.Length + startIndex);
            }

            if (length < 0)
            {
                length = Math.Max(0, str.Length - startIndex + length);
            }

            if (startIndex > str.Length - 1)
            {
                return string.Empty;
            }

            return str.Substring(startIndex, Math.Min(length, str.Length - startIndex));
        }
    }
}

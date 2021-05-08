namespace BarcodeBakery.Common
{
    /// <summary>
    /// The data input.
    /// </summary>
    /// <typeparam name="T">The type of mode.</typeparam>
    public class BCGDataInput<T>
    {
        /// <summary>
        /// Initializes the class.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="data">The data.</param>
        public BCGDataInput(T mode, string data)
        {
            Mode = mode;
            Data = data;
        }

        /// <summary>
        /// The mode.
        /// </summary>
        public T Mode
        {
            get;
        }

        /// <summary>
        /// The data.
        /// </summary>
        public string Data
        {
            get;
        }
    }
}

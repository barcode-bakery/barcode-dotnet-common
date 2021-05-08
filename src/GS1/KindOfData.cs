namespace BarcodeBakery.Common.GS1
{
    /// <summary>
    /// GS1 Kind of data.
    /// </summary>
    public enum KindOfData
    {
        /// <summary>
        /// The content is only numeric.
        /// </summary>
        Numeric,

        /// <summary>
        /// The content contains number and letters.
        /// </summary>
        Alphanumeric,

        /// <summary>
        /// The content is of a date format yymmdd.
        /// </summary>
        Date,

        /// <summary>
        /// The content is of a date and time format yymmddhhii or yymmddhhiiss.
        /// </summary>
        DateTime
    }
}

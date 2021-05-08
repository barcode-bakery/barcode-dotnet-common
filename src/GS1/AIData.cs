namespace BarcodeBakery.Common.GS1
{
    /// <summary>
    /// Entry about an AI.
    /// </summary>
    public class AIData
    {
        /// <summary>
        /// Constructor creating an entry for an AI.
        /// </summary>
        /// <param name="ai">The AI.</param>
        /// <param name="kindOfData">The type of data.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="checksum">Indicates if a checksum is present.</param>
        /// <param name="description">The description of the AI.</param>
        public AIData(string ai, KindOfData kindOfData, int minLength, int maxLength, bool checksum, string description)
        {
            AI = ai;
            KindOfData = kindOfData;
            MinLength = minLength;
            MaxLength = maxLength;
            Checksum = checksum;
            Description = description;
        }

        /// <summary>
        /// The AI.
        /// </summary>
        public string AI { get; }

        /// <summary>
        /// The type of data.
        /// </summary>
        public KindOfData KindOfData { get; }

        /// <summary>
        /// The minimum length.
        /// </summary>
        public int MinLength { get; }

        /// <summary>
        /// The maximum length.
        /// </summary>
        public int MaxLength { get; }

        /// <summary>
        /// Indicates if a checksum is required.
        /// </summary>
        public bool Checksum { get; }

        /// <summary>
        /// The description of the AI.
        /// </summary>
        public string Description { get; }
    }
}

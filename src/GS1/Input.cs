namespace BarcodeBakery.Common.GS1
{
    /// <summary>
    /// Class holding the data for GS1 data.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Constructor with no specific AI.
        /// </summary>
        /// <param name="autoContent">The content that will be parsed automatically.</param>
        public Input(string autoContent)
            : this(null, autoContent)
        {
        }

        /// <summary>
        /// Constructor with specific AI.
        /// </summary>
        /// <param name="ai">The AI.</param>
        /// <param name="content">The content associated with the AI.</param>
        public Input(string? ai, string content)
        {
            this.AI = ai;
            this.Content = content;
        }

        /// <summary>
        /// The AI.
        /// If the AI is null, the content may contain multiple AI.
        /// </summary>
        public string? AI { get; }

        /// <summary>
        /// The Content associated with the AI.
        /// If the AI is null, the content will need to be parsed.
        /// </summary>
        public string Content { get; set; }
    }
}

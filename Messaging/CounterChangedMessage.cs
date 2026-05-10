namespace Dreamine.Hybrid.Messaging
{
    /// <summary>
    /// \brief Represents a message that notifies a counter value change.
    /// </summary>
    public sealed class CounterChangedMessage : HybridMessageBase
    {
        /// <summary>
        /// \brief Initializes a new instance of the <see cref="CounterChangedMessage"/> class.
        /// </summary>
        /// <param name="count">The changed counter value.</param>
        public CounterChangedMessage(int count)
        {
            Count = count;
        }

        /// <summary>
        /// \brief Gets the changed counter value.
        /// </summary>
        public int Count { get; }
    }
}
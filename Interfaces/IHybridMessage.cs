using System;

namespace Dreamine.Hybrid.Interfaces
{
    /// <summary>
    /// Represents a message that can be exchanged between hybrid application layers.
    /// </summary>
    public interface IHybridMessage
    {
        /// <summary>
        /// Gets the unique message identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the time when the message was created.
        /// </summary>
        DateTimeOffset CreatedAt { get; }
    }
}
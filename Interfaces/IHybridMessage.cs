using System;

namespace Dreamine.Hybrid.Interfaces
{
    /// <summary>
    /// \brief Represents a message that can be exchanged between hybrid application layers.
    /// </summary>
    public interface IHybridMessage
    {
        /// <summary>
        /// \brief Gets the unique message identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// \brief Gets the time when the message was created.
        /// </summary>
        DateTimeOffset CreatedAt { get; }
    }
}
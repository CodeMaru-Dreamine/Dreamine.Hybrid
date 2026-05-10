using Dreamine.Hybrid.Interfaces;
using System;

namespace Dreamine.Hybrid.Messaging
{
    /// <summary>
    /// \brief Provides a base implementation for hybrid messages.
    /// </summary>
    public abstract class HybridMessageBase : IHybridMessage
    {
        /// <summary>
        /// \brief Initializes a new instance of the <see cref="HybridMessageBase"/> class.
        /// </summary>
        protected HybridMessageBase()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTimeOffset.Now;
        }

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt { get; }
    }
}
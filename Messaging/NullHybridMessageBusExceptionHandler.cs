using Dreamine.Hybrid.Interfaces;
using System;

namespace Dreamine.Hybrid.Messaging
{
    /// <summary>
    /// No-op implementation of <see cref="IHybridMessageBusExceptionHandler"/>.
    /// Used as the default handler when none is configured.
    /// </summary>
    public sealed class NullHybridMessageBusExceptionHandler : IHybridMessageBusExceptionHandler
    {
        /// <summary>
        /// Gets the shared singleton instance.
        /// </summary>
        public static readonly NullHybridMessageBusExceptionHandler Instance = new();

        private NullHybridMessageBusExceptionHandler() { }

        /// <inheritdoc />
        public void Handle(Exception exception, Type messageType) { }
    }
}

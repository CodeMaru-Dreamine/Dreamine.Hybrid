using System;

namespace Dreamine.Hybrid.Interfaces
{
    /// <summary>
    /// Defines a handler for exceptions thrown by individual message bus subscribers.
    /// </summary>
    /// <remarks>
    /// Implement this interface to observe subscriber failures without coupling
    /// the message bus to a concrete logging framework. Assign via
    /// <see cref="Dreamine.Hybrid.Messaging.InMemoryHybridMessageBus.ExceptionHandler"/>.
    /// The default implementation is a no-op (null object).
    /// </remarks>
    public interface IHybridMessageBusExceptionHandler
    {
        /// <summary>
        /// Called when a subscriber handler throws an unhandled exception.
        /// </summary>
        /// <param name="exception">The exception thrown by the subscriber.</param>
        /// <param name="messageType">The type of the message being published.</param>
        void Handle(Exception exception, Type messageType);
    }
}

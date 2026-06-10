using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamine.Hybrid.Interfaces
{
    /// <summary>
    /// Provides a message bus for communication between hybrid application layers.
    /// </summary>
    public interface IHybridMessageBus
    {
        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="message">The message instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous publish operation.</returns>
        Task PublishAsync<TMessage>(
            TMessage message,
            CancellationToken cancellationToken = default)
            where TMessage : IHybridMessage;

        /// <summary>
        /// Subscribes a handler to the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The message handler.</param>
        /// <returns>A disposable subscription handle.</returns>
        IDisposable Subscribe<TMessage>(
            Func<TMessage, CancellationToken, Task> handler)
            where TMessage : IHybridMessage;
    }
}
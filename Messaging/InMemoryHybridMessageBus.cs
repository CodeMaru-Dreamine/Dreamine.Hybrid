using Dreamine.Hybrid.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamine.Hybrid.Messaging
{
    /// <summary>
    /// Provides an in-memory implementation of <see cref="IHybridMessageBus"/>.
    /// </summary>
    public sealed class InMemoryHybridMessageBus : IHybridMessageBus
    {
        private readonly ConcurrentDictionary<Type, SubscriptionBucket> _subscriptions = new();
        private volatile IHybridMessageBusExceptionHandler _exceptionHandler =
            NullHybridMessageBusExceptionHandler.Instance;

        /// <summary>
        /// Gets or sets the handler invoked when a subscriber throws an unexpected exception.
        /// Defaults to <see cref="NullHybridMessageBusExceptionHandler.Instance"/> (no-op).
        /// Assign a custom handler to wire in logging without coupling this assembly to a logger.
        /// </summary>
        public IHybridMessageBusExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler;
            set => _exceptionHandler = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public async Task PublishAsync<TMessage>(
            TMessage message,
            CancellationToken cancellationToken = default)
            where TMessage : IHybridMessage
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!_subscriptions.TryGetValue(typeof(TMessage), out SubscriptionBucket? subscriptions))
            {
                return;
            }

            Subscription[] snapshot = subscriptions.CreateSnapshot();

            foreach (Subscription subscription in snapshot)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (subscription.IsDisposed)
                {
                    continue;
                }

                if (subscription.Handler is not Func<TMessage, CancellationToken, Task> handler)
                {
                    continue;
                }

                try
                {
                    await handler(message, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                catch (ObjectDisposedException)
                {
                    subscription.Dispose();
                }
                catch (InvalidOperationException)
                {
                    subscription.Dispose();
                }
                catch (NullReferenceException)
                {
                    /*
                     * Blazor Server / SignalR circuit가 아직 준비되지 않았거나,
                     * 이미 해제된 컴포넌트가 메시지를 수신할 때 발생할 수 있다.
                     *
                     * 메시지 버스는 한 구독자 실패 때문에 전체 publish를 중단하면 안 된다.
                     * 해당 구독은 더 이상 안전하지 않다고 보고 제거한다.
                     */
                    subscription.Dispose();
                }
                catch (Exception ex)
                {
                    // Individual subscriber failure must not stop delivery to remaining subscribers.
                    // Delegate to the configured handler so callers can observe the failure
                    // (e.g. by wiring a logging-backed handler) without coupling this assembly
                    // to a concrete logger. Default handler is a no-op.
                    _exceptionHandler.Handle(ex, typeof(TMessage));
                }
            }
        }

        /// <inheritdoc />
        public IDisposable Subscribe<TMessage>(
            Func<TMessage, CancellationToken, Task> handler)
            where TMessage : IHybridMessage
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Type messageType = typeof(TMessage);
            var subscription = new Subscription(messageType, handler, Unsubscribe);

            while (true)
            {
                SubscriptionBucket subscriptions = _subscriptions.GetOrAdd(
                    messageType,
                    _ => new SubscriptionBucket());

                if (subscriptions.TryAdd(subscription))
                {
                    return subscription;
                }

                _subscriptions.TryRemove(
                    new KeyValuePair<Type, SubscriptionBucket>(messageType, subscriptions));
            }
        }

        /// <summary>
        /// Removes the specified subscription.
        /// </summary>
        /// <param name="subscription">The subscription to remove.</param>
        private void Unsubscribe(Subscription subscription)
        {
            if (subscription is null)
            {
                return;
            }

            if (!_subscriptions.TryGetValue(subscription.MessageType, out SubscriptionBucket? subscriptions))
            {
                return;
            }

            subscriptions.Remove(subscription);

            if (subscriptions.TryCloseIfEmpty())
            {
                _subscriptions.TryRemove(
                    new KeyValuePair<Type, SubscriptionBucket>(subscription.MessageType, subscriptions));
            }
        }

        private sealed class SubscriptionBucket
        {
            private readonly List<Subscription> _subscriptions = new();
            private bool _closed;

            public bool TryAdd(Subscription subscription)
            {
                lock (_subscriptions)
                {
                    if (_closed)
                    {
                        return false;
                    }

                    _subscriptions.Add(subscription);
                    return true;
                }
            }

            public Subscription[] CreateSnapshot()
            {
                lock (_subscriptions)
                {
                    return _subscriptions.ToArray();
                }
            }

            public void Remove(Subscription subscription)
            {
                lock (_subscriptions)
                {
                    _subscriptions.Remove(subscription);
                }
            }

            public bool TryCloseIfEmpty()
            {
                lock (_subscriptions)
                {
                    if (_subscriptions.Count != 0)
                    {
                        return false;
                    }

                    _closed = true;
                    return true;
                }
            }
        }

        /// <summary>
        /// Represents a message subscription.
        /// </summary>
        private sealed class Subscription : IDisposable
        {
            private readonly Action<Subscription> _unsubscribe;
            private volatile bool _disposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="Subscription"/> class.
            /// </summary>
            /// <param name="messageType">The subscribed message type.</param>
            /// <param name="handler">The message handler.</param>
            /// <param name="unsubscribe">The unsubscribe callback.</param>
            public Subscription(
                Type messageType,
                object handler,
                Action<Subscription> unsubscribe)
            {
                MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
                Handler = handler ?? throw new ArgumentNullException(nameof(handler));
                _unsubscribe = unsubscribe ?? throw new ArgumentNullException(nameof(unsubscribe));
            }

            /// <summary>
            /// Gets the subscribed message type.
            /// </summary>
            public Type MessageType { get; }

            /// <summary>
            /// Gets the message handler.
            /// </summary>
            public object Handler { get; }

            /// <summary>
            /// Gets a value indicating whether this subscription has been disposed.
            /// </summary>
            public bool IsDisposed => _disposed;

            /// <inheritdoc />
            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _unsubscribe(this);
            }
        }
    }
}

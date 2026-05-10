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
    /// \brief Provides an in-memory implementation of <see cref="IHybridMessageBus"/>.
    /// </summary>
    public sealed class InMemoryHybridMessageBus : IHybridMessageBus
    {
        private readonly ConcurrentDictionary<Type, List<Subscription>> _subscriptions = new();

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

            if (!_subscriptions.TryGetValue(typeof(TMessage), out List<Subscription>? subscriptions))
            {
                return;
            }

            Subscription[] snapshot;

            lock (subscriptions)
            {
                snapshot = subscriptions.ToArray();
            }

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
                catch
                {
                    /*
                     * TODO:
                     * 이후 Dreamine.Logging 연동 시 여기서 로그를 남긴다.
                     *
                     * 현재 정책:
                     * - 메시지 버스는 fire-and-dispatch 성격이다.
                     * - 개별 구독자 예외는 전체 메시지 전파를 막지 않는다.
                     */
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
            List<Subscription> subscriptions = _subscriptions.GetOrAdd(
                messageType,
                _ => new List<Subscription>());

            var subscription = new Subscription(messageType, handler, Unsubscribe);

            lock (subscriptions)
            {
                subscriptions.Add(subscription);
            }

            return subscription;
        }

        /// <summary>
        /// \brief Removes the specified subscription.
        /// </summary>
        /// <param name="subscription">The subscription to remove.</param>
        private void Unsubscribe(Subscription subscription)
        {
            if (subscription is null)
            {
                return;
            }

            if (!_subscriptions.TryGetValue(subscription.MessageType, out List<Subscription>? subscriptions))
            {
                return;
            }

            lock (subscriptions)
            {
                subscriptions.Remove(subscription);

                if (subscriptions.Count == 0)
                {
                    _subscriptions.TryRemove(subscription.MessageType, out _);
                }
            }
        }

        /// <summary>
        /// \brief Represents a message subscription.
        /// </summary>
        private sealed class Subscription : IDisposable
        {
            private readonly Action<Subscription> _unsubscribe;
            private bool _disposed;

            /// <summary>
            /// \brief Initializes a new instance of the <see cref="Subscription"/> class.
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
            /// \brief Gets the subscribed message type.
            /// </summary>
            public Type MessageType { get; }

            /// <summary>
            /// \brief Gets the message handler.
            /// </summary>
            public object Handler { get; }

            /// <summary>
            /// \brief Gets a value indicating whether this subscription has been disposed.
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
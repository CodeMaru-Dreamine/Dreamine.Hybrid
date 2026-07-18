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
    /// \if KO
    /// <para>메시지 형식별 구독자를 스레드 안전하게 관리하는 <see cref="IHybridMessageBus"/> 인메모리 구현입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Provides an in-memory <see cref="IHybridMessageBus"/> implementation that manages subscribers by message type in a thread-safe manner.</para>
    /// \endif
    /// </summary>
    public sealed class InMemoryHybridMessageBus : IHybridMessageBus
    {
        /// <summary>
        /// \if KO
        /// <para>subscriptions 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the subscriptions value.</para>
        /// \endif
        /// </summary>
        private readonly ConcurrentDictionary<Type, SubscriptionBucket> _subscriptions = new();
        /// <summary>
        /// \if KO
        /// <para>exception Handler 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the exception handler value.</para>
        /// \endif
        /// </summary>
        private volatile IHybridMessageBusExceptionHandler _exceptionHandler =
            NullHybridMessageBusExceptionHandler.Instance;

        /// <summary>
        /// \if KO
        /// <para>구독자가 예기치 않은 예외를 발생시켰을 때 호출할 처리기를 가져오거나 설정합니다. 기본값은 무동작 <see cref="NullHybridMessageBusExceptionHandler.Instance"/>입니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the handler invoked when a subscriber throws an unexpected exception. The default is the no-op <see cref="NullHybridMessageBusExceptionHandler.Instance"/>.</para>
        /// \endif
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para>설정할 처리기가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when the assigned handler is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
        public IHybridMessageBusExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler;
            set => _exceptionHandler = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// \if KO
        /// <para>현재 스냅숏에 포함된 구독자에게 메시지를 순서대로 비동기 게시하며 개별 구독자 실패를 격리합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Asynchronously publishes a message sequentially to the current subscriber snapshot while isolating individual subscriber failures.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TMessage">
        /// \if KO
        /// <para>게시할 메시지 형식입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The message type to publish.</para>
        /// \endif
        /// </typeparam>
        /// <param name="message">
        /// \if KO
        /// <para>게시할 메시지입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The message to publish.</para>
        /// \endif
        /// </param>
        /// <param name="cancellationToken">
        /// \if KO
        /// <para>남은 전달을 중단하는 취소 토큰입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A token that stops remaining deliveries.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>현재 구독자에게 전달하는 작업입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A task representing delivery to the current subscribers.</para>
        /// \endif
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para><paramref name="message"/>가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when <paramref name="message"/> is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
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

        /// <summary>
        /// \if KO
        /// <para>지정한 메시지 형식의 비동기 처리기를 등록합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers an asynchronous handler for the specified message type.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TMessage">
        /// \if KO
        /// <para>구독할 메시지 형식입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The message type to subscribe to.</para>
        /// \endif
        /// </typeparam>
        /// <param name="handler">
        /// \if KO
        /// <para>메시지 처리 대리자입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The message-handling delegate.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>해제 시 처리기를 제거하는 구독입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A subscription that removes the handler when disposed.</para>
        /// \endif
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para><paramref name="handler"/>가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when <paramref name="handler"/> is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
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
        /// \if KO
        /// <para>지정한 구독을 메시지 형식 버킷에서 제거하고 빈 버킷을 닫습니다.</para>
        /// \endif
        /// \if EN
        /// <para>Removes the specified subscription from its message-type bucket and closes an empty bucket.</para>
        /// \endif
        /// </summary>
        /// <param name="subscription">
        /// \if KO
        /// <para>제거할 구독입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The subscription to remove.</para>
        /// \endif
        /// </param>
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

        /// <summary>
        /// \if KO
        /// <para>한 메시지 형식의 구독 컬렉션과 닫힘 상태를 잠금으로 보호합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Lock-protects the subscription collection and closed state for one message type.</para>
        /// \endif
        /// </summary>
        private sealed class SubscriptionBucket
        {
            /// <summary>
            /// \if KO
            /// <para>subscriptions 값을 보관합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Stores the subscriptions value.</para>
            /// \endif
            /// </summary>
            private readonly List<Subscription> _subscriptions = new();
            /// <summary>
            /// \if KO
            /// <para>closed 값을 보관합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Stores the closed value.</para>
            /// \endif
            /// </summary>
            private bool _closed;

            /// <summary>
            /// \if KO
            /// <para>버킷이 열려 있으면 구독을 추가합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Adds a subscription when the bucket is open.</para>
            /// \endif
            /// </summary>
            /// <param name="subscription">
            /// \if KO
            /// <para>추가할 구독입니다.</para>
            /// \endif
            /// \if EN
            /// <para>The subscription to add.</para>
            /// \endif
            /// </param>
            /// <returns>
            /// \if KO
            /// <para>추가했으면 <see langword="true"/>, 버킷이 닫혔으면 <see langword="false"/>입니다.</para>
            /// \endif
            /// \if EN
            /// <para><see langword="true"/> when added; <see langword="false"/> when the bucket is closed.</para>
            /// \endif
            /// </returns>
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

            /// <summary>
            /// \if KO
            /// <para>현재 구독 배열의 스레드 안전 스냅숏을 만듭니다.</para>
            /// \endif
            /// \if EN
            /// <para>Creates a thread-safe snapshot of the current subscription array.</para>
            /// \endif
            /// </summary>
            /// <returns>
            /// \if KO
            /// <para>현재 구독의 복사본입니다.</para>
            /// \endif
            /// \if EN
            /// <para>A copy of the current subscriptions.</para>
            /// \endif
            /// </returns>
            public Subscription[] CreateSnapshot()
            {
                lock (_subscriptions)
                {
                    return _subscriptions.ToArray();
                }
            }

            /// <summary>
            /// \if KO
            /// <para>버킷에서 지정한 구독을 제거합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Removes the specified subscription from the bucket.</para>
            /// \endif
            /// </summary>
            /// <param name="subscription">
            /// \if KO
            /// <para>제거할 구독입니다.</para>
            /// \endif
            /// \if EN
            /// <para>The subscription to remove.</para>
            /// \endif
            /// </param>
            public void Remove(Subscription subscription)
            {
                lock (_subscriptions)
                {
                    _subscriptions.Remove(subscription);
                }
            }

            /// <summary>
            /// \if KO
            /// <para>구독이 없으면 버킷을 닫아 더 이상 추가되지 않게 합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Closes the bucket when it has no subscriptions, preventing further additions.</para>
            /// \endif
            /// </summary>
            /// <returns>
            /// \if KO
            /// <para>빈 버킷을 닫았으면 <see langword="true"/>, 아직 구독이 있으면 <see langword="false"/>입니다.</para>
            /// \endif
            /// \if EN
            /// <para><see langword="true"/> when the empty bucket was closed; <see langword="false"/> when subscriptions remain.</para>
            /// \endif
            /// </returns>
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
        /// \if KO
        /// <para>메시지 형식과 처리기 및 구독 해제 콜백을 보유하는 구독을 나타냅니다.</para>
        /// \endif
        /// \if EN
        /// <para>Represents a subscription holding its message type, handler, and unsubscribe callback.</para>
        /// \endif
        /// </summary>
        private sealed class Subscription : IDisposable
        {
            /// <summary>
            /// \if KO
            /// <para>unsubscribe 값을 보관합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Stores the unsubscribe value.</para>
            /// \endif
            /// </summary>
            private readonly Action<Subscription> _unsubscribe;
            /// <summary>
            /// \if KO
            /// <para>disposed 값을 보관합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Stores the disposed value.</para>
            /// \endif
            /// </summary>
            private volatile bool _disposed;

            /// <summary>
            /// \if KO
            /// <para>메시지 형식, 처리기 및 구독 해제 콜백으로 새 구독을 초기화합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Initializes a new subscription with its message type, handler, and unsubscribe callback.</para>
            /// \endif
            /// </summary>
            /// <param name="messageType">
            /// \if KO
            /// <para>구독한 메시지 형식입니다.</para>
            /// \endif
            /// \if EN
            /// <para>The subscribed message type.</para>
            /// \endif
            /// </param>
            /// <param name="handler">
            /// \if KO
            /// <para>메시지 처리기 객체입니다.</para>
            /// \endif
            /// \if EN
            /// <para>The message-handler object.</para>
            /// \endif
            /// </param>
            /// <param name="unsubscribe">
            /// \if KO
            /// <para>해제 시 호출할 콜백입니다.</para>
            /// \endif
            /// \if EN
            /// <para>The callback invoked during disposal.</para>
            /// \endif
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// \if KO
            /// <para>인수 중 하나가 <see langword="null"/>인 경우 발생합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Thrown when any argument is <see langword="null"/>.</para>
            /// \endif
            /// </exception>
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
            /// \if KO
            /// <para>구독한 메시지 형식을 가져옵니다.</para>
            /// \endif
            /// \if EN
            /// <para>Gets the subscribed message type.</para>
            /// \endif
            /// </summary>
            public Type MessageType { get; }

            /// <summary>
            /// \if KO
            /// <para>메시지 처리기 객체를 가져옵니다.</para>
            /// \endif
            /// \if EN
            /// <para>Gets the message-handler object.</para>
            /// \endif
            /// </summary>
            public object Handler { get; }

            /// <summary>
            /// \if KO
            /// <para>이 구독이 해제되었는지 여부를 가져옵니다.</para>
            /// \endif
            /// \if EN
            /// <para>Gets whether this subscription has been disposed.</para>
            /// \endif
            /// </summary>
            public bool IsDisposed => _disposed;

            /// <summary>
            /// \if KO
            /// <para>구독을 한 번만 해제하고 소유 버스에서 제거합니다.</para>
            /// \endif
            /// \if EN
            /// <para>Disposes the subscription at most once and removes it from the owning bus.</para>
            /// \endif
            /// </summary>
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

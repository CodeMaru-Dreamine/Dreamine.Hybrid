/// \file InMemoryHybridMessageBus.cs
/// \brief 인메모리 기반 메시지 버스 구현.
/// \author Dreamine
/// \date 2026-01-28
/// \version 1.0.0
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
namespace Dreamine.Hybrid.Messaging
{
    /// <summary>단순 인메모리 메시지 버스 구현입니다.</summary>
    public sealed class InMemoryHybridMessageBus : IHybridMessageBus
    {
        private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

        /// <inheritdoc/>
        public void Publish<TMessage>(TMessage message) where TMessage : IHybridMessage
        {
            if (message is null) throw new ArgumentNullException(nameof(message));
            if (_handlers.TryGetValue(typeof(TMessage), out var list) == false) return;

            Delegate[] snapshot;
            lock (list) snapshot = list.ToArray();

            foreach (var d in snapshot)
            {
                if (d is Action<TMessage> h) h(message);
            }
        }

        /// <inheritdoc/>
        public IDisposable Subscribe<TMessage>(Action<TMessage> handler) where TMessage : IHybridMessage
        {
            if (handler is null) throw new ArgumentNullException(nameof(handler));
            var list = _handlers.GetOrAdd(typeof(TMessage), _ => new List<Delegate>());
            lock (list) list.Add(handler);

            return new Subscription(() =>
            {
                if (_handlers.TryGetValue(typeof(TMessage), out var target) == false) return;
                lock (target) target.Remove(handler);
            });
        }

        private sealed class Subscription : IDisposable
        {
            private readonly Action _dispose;
            private bool _disposed;
            public Subscription(Action dispose) => _dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));
            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _dispose();
            }
        }
    }
}

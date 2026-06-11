using Dreamine.Hybrid.Interfaces;
using System;
using System.Threading;

namespace Dreamine.Hybrid.State
{
    /// <summary>
    /// Provides an in-memory shared state store for hybrid applications.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    public sealed class HybridStateStore<TState> : IHybridStateStore<TState>
    {
        private readonly object _syncRoot = new();
        private TState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="HybridStateStore{TState}"/> class.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        public HybridStateStore(TState initialState)
        {
            _state = initialState;
        }

        /// <inheritdoc />
        public event EventHandler<HybridStateChangedEventArgs<TState>>? StateChanged;

        /// <inheritdoc />
        public IDisposable Subscribe(EventHandler<HybridStateChangedEventArgs<TState>> handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            StateChanged += handler;
            return new StateSubscription(this, handler);
        }

        /// <inheritdoc />
        public TState State
        {
            get
            {
                lock (_syncRoot)
                {
                    return _state;
                }
            }
        }

        /// <inheritdoc />
        public void SetState(TState state)
        {
            TState snapshot;

            lock (_syncRoot)
            {
                _state = state;
                // Capture the snapshot inside the lock so the event argument always
                // reflects the stored value at the time of assignment, even when
                // concurrent SetState calls race. Subscribers receive the state that
                // was actually committed, not a stale caller-held reference.
                snapshot = _state;
            }

            OnStateChanged(snapshot);
        }

        /// <inheritdoc />
        public void Update(Func<TState, TState> updater)
        {
            if (updater is null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            TState snapshot;

            lock (_syncRoot)
            {
                snapshot = updater(_state);
                _state = snapshot;
            }

            OnStateChanged(snapshot);
        }

        /// <summary>
        /// Raises the state changed event.
        /// </summary>
        /// <param name="state">The changed state.</param>
        private void OnStateChanged(TState state)
        {
            StateChanged?.Invoke(this, new HybridStateChangedEventArgs<TState>(state));
        }

        private sealed class StateSubscription : IDisposable
        {
            private readonly HybridStateStore<TState> _store;
            private EventHandler<HybridStateChangedEventArgs<TState>>? _handler;

            public StateSubscription(
                HybridStateStore<TState> store,
                EventHandler<HybridStateChangedEventArgs<TState>> handler)
            {
                _store = store;
                _handler = handler;
            }

            public void Dispose()
            {
                // Interlocked.Exchange ensures the handler is cleared and unsubscribed
                // exactly once even when Dispose is called concurrently.
                var handler = Interlocked.Exchange(ref _handler, null);
                if (handler is null)
                {
                    return;
                }

                _store.StateChanged -= handler;
            }
        }
    }
}

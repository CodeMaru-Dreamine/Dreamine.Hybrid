using Dreamine.Hybrid.Interfaces;
using System;

namespace Dreamine.Hybrid.State
{
    /// <summary>
    /// \brief Provides an in-memory shared state store for hybrid applications.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    public sealed class HybridStateStore<TState> : IHybridStateStore<TState>
    {
        private readonly object _syncRoot = new();
        private TState _state;

        /// <summary>
        /// \brief Initializes a new instance of the <see cref="HybridStateStore{TState}"/> class.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        public HybridStateStore(TState initialState)
        {
            _state = initialState;
        }

        /// <inheritdoc />
        public event EventHandler<HybridStateChangedEventArgs<TState>>? StateChanged;

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
            lock (_syncRoot)
            {
                _state = state;
            }

            OnStateChanged(state);
        }

        /// <inheritdoc />
        public void Update(Func<TState, TState> updater)
        {
            if (updater is null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            TState newState;

            lock (_syncRoot)
            {
                newState = updater(_state);
                _state = newState;
            }

            OnStateChanged(newState);
        }

        /// <summary>
        /// \brief Raises the state changed event.
        /// </summary>
        /// <param name="state">The changed state.</param>
        private void OnStateChanged(TState state)
        {
            StateChanged?.Invoke(this, new HybridStateChangedEventArgs<TState>(state));
        }
    }
}
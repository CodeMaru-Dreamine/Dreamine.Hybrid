using Dreamine.Hybrid.State;
using System;

namespace Dreamine.Hybrid.Interfaces
{
    /// <summary>
    /// Provides a shared state container for hybrid WPF and Blazor contexts.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    public interface IHybridStateStore<TState>
    {
        /// <summary>
        /// Occurs when the state has changed.
        /// </summary>
        event EventHandler<HybridStateChangedEventArgs<TState>>? StateChanged;

        /// <summary>
        /// Subscribes to state changes and returns a disposable unsubscriber.
        /// </summary>
        /// <param name="handler">The state change handler.</param>
        /// <returns>A disposable subscription that removes the handler when disposed.</returns>
        IDisposable Subscribe(EventHandler<HybridStateChangedEventArgs<TState>> handler);

        /// <summary>
        /// Gets the current state.
        /// </summary>
        TState State { get; }

        /// <summary>
        /// Replaces the current state.
        /// </summary>
        /// <param name="state">The new state.</param>
        void SetState(TState state);

        /// <summary>
        /// Updates the current state using the specified updater function.
        /// </summary>
        /// <param name="updater">The state updater function.</param>
        void Update(Func<TState, TState> updater);
    }
}

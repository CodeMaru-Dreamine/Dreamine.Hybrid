using Dreamine.Hybrid.State;
using System;

namespace Dreamine.Hybrid.Interfaces
{
    /// <summary>
    /// \brief Provides a shared state container for hybrid WPF and Blazor contexts.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    public interface IHybridStateStore<TState>
    {
        /// <summary>
        /// \brief Occurs when the state has changed.
        /// </summary>
        event EventHandler<HybridStateChangedEventArgs<TState>>? StateChanged;

        /// <summary>
        /// \brief Gets the current state.
        /// </summary>
        TState State { get; }

        /// <summary>
        /// \brief Replaces the current state.
        /// </summary>
        /// <param name="state">The new state.</param>
        void SetState(TState state);

        /// <summary>
        /// \brief Updates the current state using the specified updater function.
        /// </summary>
        /// <param name="updater">The state updater function.</param>
        void Update(Func<TState, TState> updater);
    }
}
using System;

namespace Dreamine.Hybrid.State
{
    /// <summary>
    /// \brief Provides event data for hybrid state changes.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    public sealed class HybridStateChangedEventArgs<TState> : EventArgs
    {
        /// <summary>
        /// \brief Initializes a new instance of the <see cref="HybridStateChangedEventArgs{TState}"/> class.
        /// </summary>
        /// <param name="state">The changed state.</param>
        public HybridStateChangedEventArgs(TState state)
        {
            State = state;
        }

        /// <summary>
        /// \brief Gets the changed state.
        /// </summary>
        public TState State { get; }
    }
}
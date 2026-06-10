using System;

namespace Dreamine.Hybrid.State
{
    /// <summary>
    /// Provides event data for hybrid state changes.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    public sealed class HybridStateChangedEventArgs<TState> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HybridStateChangedEventArgs{TState}"/> class.
        /// </summary>
        /// <param name="state">The changed state.</param>
        public HybridStateChangedEventArgs(TState state)
        {
            State = state;
        }

        /// <summary>
        /// Gets the changed state.
        /// </summary>
        public TState State { get; }
    }
}
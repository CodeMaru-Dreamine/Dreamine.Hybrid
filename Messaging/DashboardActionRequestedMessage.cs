namespace Dreamine.Hybrid.Messaging
{
    /// <summary>
    /// \brief Represents a message that requests a dashboard action.
    /// </summary>
    public sealed class DashboardActionRequestedMessage : HybridMessageBase
    {
        /// <summary>
        /// \brief Initializes a new instance of the <see cref="DashboardActionRequestedMessage"/> class.
        /// </summary>
        /// <param name="action">The requested dashboard action.</param>
        public DashboardActionRequestedMessage(DashboardAction action)
        {
            Action = action;
        }

        /// <summary>
        /// \brief Gets the requested dashboard action.
        /// </summary>
        public DashboardAction Action { get; }
    }
}
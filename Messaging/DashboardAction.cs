namespace Dreamine.Hybrid.Messaging
{
    /// <summary>
    /// \brief Defines actions that can be requested from the dashboard.
    /// </summary>
    public enum DashboardAction
    {
        /// <summary>
        /// \brief No action.
        /// </summary>
        None = 0,

        /// <summary>
        /// \brief Requests opening the project view.
        /// </summary>
        OpenProject = 1,

        /// <summary>
        /// \brief Requests opening the NuGet view.
        /// </summary>
        OpenNuget = 2,

        /// <summary>
        /// \brief Requests opening the documentation view.
        /// </summary>
        OpenDocs = 3,

        /// <summary>
        /// \brief Requests opening the settings view.
        /// </summary>
        OpenSettings = 4,

        /// <summary>
        /// \brief Requests opening the embedded view.
        /// </summary>
        OpenEmbedded = 5,

        /// <summary>
        /// \brief Requests opening the external browser view.
        /// </summary>
        OpenExternalBrowser = 6,

        /// <summary>
        /// \brief Requests opening the server mode view.
        /// </summary>
        OpenServerMode = 7,

        /// <summary>
        /// \brief Requests resetting the counter.
        /// </summary>
        ResetCounter = 8
    }
}
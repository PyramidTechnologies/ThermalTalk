namespace ThermalTalk
{
    /// <summary>
    /// Interface for defining a logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Called for the debug log level
        /// </summary>
        /// <param name="log">string of information to log</param>
        void Debug(string log);

        /// <summary>
        /// Called for the Warn log level
        /// </summary>
        /// <param name="log">string of information to log</param>
        void Warn(string log);

        /// <summary>
        /// Called for the Error log level
        /// </summary>
        /// <param name="log">string of information to log</param>
        void Error(string log);

        /// <summary>
        /// Called for the Info log level
        /// </summary>
        /// <param name="log">string of information to log</param>
        void Info(string log);

        /// <summary>
        /// Called for the Trace log level
        /// </summary>
        /// <param name="log">string of information to log</param>
        void Trace(string log);
    }
}
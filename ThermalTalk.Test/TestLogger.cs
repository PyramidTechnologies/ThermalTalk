namespace ThermalTalk.Test
{
    using System.Collections.Generic;

    /// <summary>
    /// Test logger to be used for unit tests
    /// </summary>
    public class TestLogger : ILogger
    {
        public Queue<string> DebugOutput = new Queue<string>();
        public Queue<string> WarnOutput = new Queue<string>();
        public Queue<string> ErrorOutput = new Queue<string>();
        public Queue<string> InfoOutput = new Queue<string>();
        public Queue<string> TraceOutput = new Queue<string>();
        
        /// <summary>
        /// Enqueue mock debug logs
        /// </summary>
        /// <param name="log">log to be stored</param>
        public void Debug(string log)
        {
            DebugOutput.Enqueue(log);
        }

        /// <summary>
        /// Enqueue mock warn logs
        /// </summary>
        /// <param name="log">log to be stored</param>
        public void Warn(string log)
        {
            WarnOutput.Enqueue(log);
        }

        /// <summary>
        /// Enqueue mock error logs
        /// </summary>
        /// <param name="log">log to be stored</param>
        public void Error(string log)
        {
            ErrorOutput.Enqueue(log);
        }

        /// <summary>
        /// Enqueue mock info logs
        /// </summary>
        /// <param name="log">log to be stored</param>
        public void Info(string log)
        {
            InfoOutput.Enqueue(log);
        }

        /// <summary>
        /// Enqueue mock trace logs
        /// </summary>
        /// <param name="log">log to be stored</param>
        public void Trace(string log)
        {
            TraceOutput.Enqueue(log);
        }
    }
}
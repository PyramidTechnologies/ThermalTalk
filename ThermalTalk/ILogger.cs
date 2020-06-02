namespace ThermalTalk
{
    public interface ILogger
    {
        void Debug(string log);

        void Warn(string log);

        void Error(string log);

        void Info(string log);

        void Trace(string log);
    }
}
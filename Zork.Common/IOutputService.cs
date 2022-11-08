namespace Zork.Common
{
    public interface IOutputService
    {
        void Write(object obj);

        void WriteLine(object obj);

        void Write(string message);

        void WriteLine(string message);
    }
}

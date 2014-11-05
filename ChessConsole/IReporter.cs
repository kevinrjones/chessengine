namespace ChessConsole
{
    public interface IReporter
    {
        void Report(int count);
        void Report(string message);
    }
}
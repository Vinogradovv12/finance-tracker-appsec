namespace FinanceTracker.Api.Common;

public interface IAppLogger
{
    void Info (string message);
    void Warning (string message);
    void Error (string message);
}
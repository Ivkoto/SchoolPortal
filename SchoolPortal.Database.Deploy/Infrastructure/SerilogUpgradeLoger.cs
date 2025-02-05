using DbUp.Engine.Output;
using Serilog;

namespace SchoolPortal.Database.Deploy.Infrastructure;

public class SerilogUpgradeLoger : IUpgradeLog
{
    private readonly ILogger logger;

    public SerilogUpgradeLoger(ILogger logger)
    {
        this.logger = logger;
    }

    public void LogInformation(string format, params object[] args)
    {
        logger.Information(format, args);
    }

    public void LogError(string format, params object[] args)
    {
        logger.Error(format, args);
    }

    public void LogError(Exception ex, string format, params object[] args)
    {
        logger.Error(format, format, args);
    }

    public void LogWarning(string format, params object[] args)
    {
        logger.Warning(format, args);
    }

    public void LogDebug(string format, params object[] args)
    {
        logger.Debug(format, args);
    }

    public void LogTrace(string format, params object[] args)
    {
        logger.Verbose(format, args);
    }
}

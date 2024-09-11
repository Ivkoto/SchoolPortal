using DbUp.Engine.Output;
using Serilog;

namespace SchoolPortal.Database.Deploy.OLD.Logging
{
    internal class SerilogUpgradeLog : IUpgradeLog
    {
        public void WriteError(string format, params object[] args) => Log.Logger.Error(string.Format(format, args));

        public void WriteInformation(string format, params object[] args) => Log.Logger.Information(string.Format(format, args));

        public void WriteWarning(string format, params object[] args) => Log.Logger.Warning(string.Format(format, args));
    }
}

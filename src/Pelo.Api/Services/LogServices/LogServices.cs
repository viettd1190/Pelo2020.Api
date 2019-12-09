using NLog;
using Pelo.Common.Extensions;

namespace Pelo.Api.Services.LogServices
{
    public interface ILogService
    {
        void WriteLog(object obj);
    }

    public class LogService : ILogService
    {
        private readonly Logger _logger;

        public LogService()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        #region ILogService Members

        public void WriteLog(object obj)
        {
            _logger.Info(obj.ToJson());
        }

        #endregion
    }
}
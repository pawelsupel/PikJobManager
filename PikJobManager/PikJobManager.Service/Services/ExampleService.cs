using NLog;
using NLog.Web;

namespace PikJobManager.Service.Services
{
    public class ExampleService : IExampleService
    {
        private readonly ILogger logger;
        private readonly ILogger classLogger;
        
        public ExampleService(ILogger logger)
        {
            this.logger = logger;
            this.classLogger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }

        public void Log()
        {
            this.logger.Info("*");
            this.classLogger.Info("*");
        }
    }
}
using NLog;

namespace PikJobManager.Service.Services
{
    public class ExampleService : IExampleService
    {
        private readonly ILogger logger;
        
        public ExampleService(ILogger logger)
        {
            this.logger = logger;
        }

        public void Log()
        {
            this.logger.Info("*");
        }
    }
}
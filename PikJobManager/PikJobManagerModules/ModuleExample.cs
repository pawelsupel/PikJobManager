using System;
using System.Threading.Tasks;
using NLog;
using PikJobManager.Core;
using Quartz;

namespace PikJobManagerModules
{
    [DisallowConcurrentExecution]
    public class ModuleExample : ModuleBase, IPikJobManagerModule
    {
        private readonly ILogger logger;
        public ModuleExample(ILogger logger)
        {
            this.logger = logger;
        }
        
        public Task Execute(IJobExecutionContext context)
        {
            this.logger.Info("*");
            
            return Task.CompletedTask;
        }
    }
}
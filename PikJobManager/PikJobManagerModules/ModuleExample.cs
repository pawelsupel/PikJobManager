using System.Threading.Tasks;
using PikJobManager.Core;
using PikJobManager.Service.Services;
using Quartz;

namespace PikJobManagerModules
{
    [DisallowConcurrentExecution]
    public class ModuleExample : ModuleBase, IPikJobManagerModule
    {
        private readonly IExampleService service;
        public ModuleExample(IExampleService service)
        {
            this.service = service;
        }
        
        public Task Execute(IJobExecutionContext context)
        {
            this.service.Log();
            
            return Task.CompletedTask;
        }
    }
}
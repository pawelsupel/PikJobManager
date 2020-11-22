using Autofac;
using PikJobManager.Core;
using PikJobManager.Service.Services;

namespace PikJobManagerModules
{
    public class AutofacExampleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ExampleService>().As<IExampleService>();
        }
    }
}
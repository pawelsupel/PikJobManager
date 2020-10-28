using Autofac;
using PikJobManagerModules.Services;

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
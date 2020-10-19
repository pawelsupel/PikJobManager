using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.Quartz;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
using PikJobManager.Core;
using Quartz;
using Quartzmin;

namespace PikJobManager.App
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        private IContainer container { get; set; }
        private readonly ILogger logger;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            
            this.logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddQuartzmin();
            
            services.AddOptions();
            services.Configure<SchedulerSettings>(options => Configuration.GetSection("SchedulerSettings").Bind(options));

            var schedulerSettings = Configuration.GetSection("SchedulerSettings").Get<SchedulerSettings>();
            
            var assemblies = new List<Assembly>();
            var modulesApp = $"{System.AppDomain.CurrentDomain.BaseDirectory}/Modules";
            foreach (string assemblyPath in Directory.GetFiles(modulesApp, "*Modules.dll", SearchOption.TopDirectoryOnly))
            {
                var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                assemblies.Add(assembly);
            }

            var typesToRegister = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var items = assembly.GetTypes()
                    .Where(x => x.IsClass && x.IsPublic && x.GetInterfaces().Contains(typeof(IPikJobManagerModule))).ToList();
                
                typesToRegister.AddRange(items);
            }
            
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterInstance(this.logger).As<ILogger>();

            ConfigureContainer(containerBuilder, typesToRegister);
            
            this.container = containerBuilder.Build();
            
            var scheduler = container.Resolve<IScheduler>();
            TriggerJobs(scheduler, schedulerSettings, typesToRegister);
            scheduler.Start();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            var scheduler = this.container.Resolve<IScheduler>();
        
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseQuartzmin(new QuartzminOptions() { Scheduler = scheduler, VirtualPathRoot = "/" , UseLocalTime =true, DefaultDateFormat="yyyy-MM-dd", DefaultTimeFormat="HH:mm:ss \"GMT\"zzz" });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
        
        internal static ContainerBuilder ConfigureContainer(ContainerBuilder cb, List<Type> types)
        {
            // 1) Register IScheduler
            cb.RegisterModule(new QuartzAutofacFactoryModule()); 
            // 2) Register jobs
            foreach (var typeR in types)
            {
                cb.RegisterModule(new QuartzAutofacJobsModule(typeR.Assembly));
            }

            return cb;
        }

        internal static void TriggerJobs(IScheduler scheduler, SchedulerSettings settings, List<Type> types)
        {
            foreach (var jobSchedule in settings.JobSettingses)
            {
                var job = CreateJob(jobSchedule, types);
                var trigger = CreateTrigger(jobSchedule,types);

                scheduler.ScheduleJob(job, trigger);
            }
        }
        
        private static IJobDetail CreateJob(JobSettings schedule, List<Type> types)
        {
            var jobType = types.FirstOrDefault(x => x.FullName == schedule.FullName);

            if (jobType == null)
                return null;
            
            return JobBuilder
                .Create(jobType)
                .WithIdentity(schedule.JobKey)
                .WithDescription(jobType.Name)
                .Build();
        }

        private static ITrigger CreateTrigger(JobSettings schedule, List<Type> types)
        {
            var jobType = types.FirstOrDefault(x => x.FullName == schedule.FullName);

            if (jobType == null)
                return null;
            
            return TriggerBuilder
                .Create()
                .WithIdentity($"{jobType.FullName}.trigger")
                .WithCronSchedule(schedule.Cron)
                .WithDescription(schedule.Cron)
                .Build();
        }
    }
}
using System.Collections.Generic;

namespace PikJobManager.App
{
    public class SchedulerSettings
    {
        public List<JobSettings> JobSettingses { get; set; } 
    }

    public class JobSettings
    {
        public string FullName { get; set; }
        public string Cron { get; set; }
        public string JobKey { get; set; } 
    }
}
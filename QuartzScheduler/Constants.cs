using Quartz;

namespace QuartzScheduler
{
    public static class Constants
    {
        public static readonly JobKey JobKey = new("LogJob");
        public static readonly TriggerKey TriggerKey = new("LogJob-trigger");
    }
}

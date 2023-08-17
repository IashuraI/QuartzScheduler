using Quartz;

namespace AmarenderReddy
{
    public static class Constants
    {
        public static readonly JobKey JobKey = new("LogJob");
        public static readonly TriggerKey TriggerKey = new("LogJob-trigger");
    }
}

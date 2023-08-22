using Quartz;

namespace QuartzScheduler.Services
{
    public class TriggerCalculationService
    {
        public int CalculateAmountOfJobNeedsToBeDone(DateTimeOffset lastTriggerTime, string cronString)
        {
            CronExpression cronExpression = new CronExpression(cronString);
            DateTimeOffset? nextTriggerTime = lastTriggerTime;
            int amount = 0;

            while (nextTriggerTime != null)
            {
                nextTriggerTime = cronExpression.GetNextValidTimeAfter(nextTriggerTime.Value);

                if (DateTimeOffset.Compare(nextTriggerTime!.Value, DateTimeOffset.UtcNow) <= 0)
                {
                    amount++;
                }
                else
                {
                    break;
                }
            }

            return amount;
        }
    }
}

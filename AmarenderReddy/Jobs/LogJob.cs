using AmarenderReddy.Data;
using AmarenderReddy.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace AmarenderReddy.Jobs
{
    public class LogJob : IJob
    {
        private readonly ILogger<LogJob> _logger;
        private readonly DummyContext _dummyContext;
        private readonly TriggerCalculationService _triggerCalculationService;

        public LogJob(ILogger<LogJob> logger, DummyContext dummyContext, TriggerCalculationService triggerCalculationService)
        {
            _logger = logger;
            _dummyContext = dummyContext;
            _triggerCalculationService = triggerCalculationService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Hello world!");

            DummyModel? dummyModel = await _dummyContext.DummyModel.FirstOrDefaultAsync();
            if (dummyModel is not null)
            {
                if (dummyModel.LastTriggerTime.HasValue)
                {
                    //trigger job all the times it was not triggered
                    int triggerAmount = _triggerCalculationService.CalculateAmountOfJobNeedsToBeDone(dummyModel.LastTriggerTime.Value, dummyModel.Cron);
                    for (int i = 0; i < triggerAmount; i++)
                    {
                        await context.Scheduler.TriggerJob(Constants.JobKey);
                    }
                }

                dummyModel.LastTriggerTime = DateTimeOffset.UtcNow;
                await _dummyContext.SaveChangesAsync();
            }
            
            await Task.CompletedTask;
        }
    }
}

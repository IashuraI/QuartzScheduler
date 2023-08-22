using QuartzScheduler.Data;
using QuartzScheduler.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace QuartzScheduler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DummyModelController : ControllerBase
    {   
        private readonly IScheduler _scheduler;
        private readonly DummyContext _dummyContext;
        private readonly TriggerCalculationService _triggerCalculationService;

        public DummyModelController(ISchedulerFactory schedulerFactory, DummyContext dummyContext, TriggerCalculationService triggerCalculationService)
        {
            _scheduler = schedulerFactory.GetScheduler().Result;
            _dummyContext = dummyContext;
            _triggerCalculationService = triggerCalculationService;
        }

        [HttpPost]
        public async Task UpdateCron(DummyModel dummyModel)
        {
            DummyModel? dummyModelDb = await _dummyContext.DummyModel.FirstOrDefaultAsync(x => x.Id == dummyModel.Id);

            if (dummyModelDb != null)
            {
                if (dummyModel.Publish)
                {
                    ITrigger newTrigger = TriggerBuilder.Create()
                        .ForJob(Constants.JobKey)
                        .WithIdentity(Constants.TriggerKey)
                        .WithCronSchedule(dummyModel.Cron)
                        .Build();

                    ITrigger? oldTrigger = await _scheduler.GetTrigger(Constants.TriggerKey);
                    if (oldTrigger != null && dummyModelDb.LastTriggerTime.HasValue)
                    {
                        //trigger job all the times it was not triggered
                        int triggerAmount = _triggerCalculationService.CalculateAmountOfJobNeedsToBeDone(dummyModelDb.LastTriggerTime.Value, dummyModelDb.Cron);
                        for (int i = 0; i < triggerAmount; i++)
                        {
                            await _scheduler.TriggerJob(Constants.JobKey);
                        }

                        await _scheduler.RescheduleJob(Constants.TriggerKey, newTrigger);
                    }
                    else
                    {
                        await _scheduler.ScheduleJob(newTrigger);
                    }
                }
                else
                {
                    await _scheduler.UnscheduleJob(Constants.TriggerKey);
                }

                dummyModelDb.Publish = dummyModel.Publish;
                dummyModelDb.Name = dummyModel.Name;
                dummyModelDb.Cron = dummyModel.Cron;
            }
            else
            {
                ITrigger? oldTrigger = await _scheduler.GetTrigger(Constants.TriggerKey);

                if (dummyModel.Publish)
                {
                    ITrigger trigger = TriggerBuilder.Create()
                        .ForJob(Constants.JobKey)
                        .WithIdentity(Constants.TriggerKey)
                        .WithCronSchedule(dummyModel.Cron)
                        .Build();

                    if(oldTrigger != null)
                    {
                        await _scheduler.RescheduleJob(Constants.TriggerKey, trigger);
                    }
                    else
                    {
                        await _scheduler.ScheduleJob(trigger);
                    }

                    _dummyContext.DummyModel.Add(dummyModel);
                }
                else
                {
                    if (oldTrigger != null)
                    {
                        await _scheduler.UnscheduleJob(Constants.TriggerKey);
                    }
                }
            }

            _dummyContext.SaveChanges();
        }
    }
}
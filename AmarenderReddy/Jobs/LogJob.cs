using AmarenderReddy.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace AmarenderReddy.Jobs
{
    public class LogJob : IJob
    {
        private readonly ILogger<LogJob> _logger;
        private readonly DummyContext _dummyContext;
        public LogJob(ILogger<LogJob> logger, DummyContext dummyContext)
        {
            _logger = logger;
            _dummyContext = dummyContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Hello world!");

            DummyModel? dummyModel = await _dummyContext.DummyModel.FirstOrDefaultAsync();
            if (dummyModel is not null)
            {
                dummyModel.LastTriggerTime = DateTimeOffset.UtcNow;
                await _dummyContext.SaveChangesAsync();
            }
            
            await Task.CompletedTask;
        }
    }
}

using AmarenderReddy;
using AmarenderReddy.Data;
using AmarenderReddy.Jobs;
using AmarenderReddy.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DummyContext>(options =>
            options.UseNpgsql("Host=localhost;Database=dummy;Username=postgres;Password=Password"));

builder.Services.AddQuartz(q =>
{
    // base Quartz scheduler, job and trigger configuration
    q.UsePersistentStore(store =>
    {
        // it's generally recommended to stick with
        // string property keys and values when serializing
        store.UseProperties = true;
        store.UsePostgres(db =>
            db.ConnectionString = "Host=localhost;Database=dummy;Username=postgres;Password=Password"
        );

        store.UseNewtonsoftJsonSerializer();
    });
    // Just use the name of your job that you created in the Jobs folder.
    q.AddJob<LogJob>(opts => opts.WithIdentity(Constants.JobKey));

    q.AddTrigger(opts => opts
        .ForJob(Constants.JobKey)
        .WithIdentity(Constants.TriggerKey)
         .StartNow()
        .WithSimpleSchedule(x => x
  .WithIntervalInSeconds(10)
  .RepeatForever())
    );
});

// ASP.NET Core hosting
builder.Services.AddQuartzServer(options =>
{
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
});

builder.Services.AddTransient<TriggerCalculationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

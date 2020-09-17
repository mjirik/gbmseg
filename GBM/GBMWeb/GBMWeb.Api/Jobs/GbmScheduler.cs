using Quartz.Impl;
using System.Collections.Specialized;
using Quartz;

namespace GBMWeb.Api.Jobs
{
    public class GbmScheduler
    {
        public async void Start()
        {
            //Log.Information("Starting scheduler");

            var props = new NameValueCollection()
            {
                {"quartz.serializer.type", "binary"}
            };

            var factory = new StdSchedulerFactory(props);

            var scheduler = await factory.GetScheduler();
            await scheduler.Start();

            InitiateJobs(scheduler);

            //Log.Information("Scheduler started");
        }

        private static async void InitiateJobs(IScheduler scheduler)
        {
            var processImageJob = JobBuilder.Create<ProcessImageJob>()
                .WithIdentity(ProcessImageJob.JobIdentity, ProcessImageJob.GroupIdentity)
                .Build();

            var processImageTrigger = TriggerBuilder.Create()
                .WithIdentity(ProcessImageJob.TriggerIdentity, ProcessImageJob.GroupIdentity)
                .StartNow()
                .WithSimpleSchedule(x => x.WithInterval(ProcessImageJob.Interval).RepeatForever())
                .Build();

            await scheduler.ScheduleJob(processImageJob, processImageTrigger);

            var measureImageJob = JobBuilder.Create<MeasureImageJob>()
                .WithIdentity(MeasureImageJob.JobIdentity, MeasureImageJob.GroupIdentity)
                .Build();

            var measureImageTrigger = TriggerBuilder.Create()
                .WithIdentity(MeasureImageJob.TriggerIdentity, MeasureImageJob.GroupIdentity)
                .StartNow()
                .WithSimpleSchedule(x => x.WithInterval(MeasureImageJob.Interval).RepeatForever())
                .Build();

            await scheduler.ScheduleJob(measureImageJob, measureImageTrigger);
        }
    }
}

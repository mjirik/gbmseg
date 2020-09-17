using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GBMWeb.Data.Contexts;
using GBMWeb.Data.Models;
using GBMWeb.Shared;
using Quartz;

namespace GBMWeb.Api.Jobs
{
    public class ProcessImageJob : IJob
    {
        #region Job Settings

        public const string JobIdentity = "ProcessImageJob";
        public const string TriggerIdentity = "ProcessImageTrigger";
        public const string GroupIdentity = "ProcessImageGroup";
        public static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

        #endregion

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Executing ProcessImageJob.");

            using var dbContext = new GbmDbContext();

            foreach (var task in dbContext.Tasks.Where(t => t.Status == (int)MeasureTaskStatus.Created))
            {

                dbContext.Tasks.Single(t => t.Id == task.Id).Status = (int)MeasureTaskStatus.InProcess;
                dbContext.SaveChanges();

                var args =
                    $"{ApplicationContext.Current.Configuration["PythonRepository"]}/process.py {ApplicationContext.Current.Configuration["PythonRepository"]}/settings.json {task.InputImageFilePath} {task.OutputImageFilePath}";

                Console.WriteLine($"Processing task {task.Id}");
                Console.WriteLine($"{ApplicationContext.Current.Configuration["PythonRuntime"]} {args}");

                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = ApplicationContext.Current.Configuration["PythonRuntime"],
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine(result);
                    }
                }

                dbContext.Tasks.Single(t => t.Id == task.Id).Status = (int)MeasureTaskStatus.Segmented;
                dbContext.SaveChanges();
            }

            Console.WriteLine("ProcessImageJob finished.");
            return Task.CompletedTask;
        }
    }
}

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
    public class MeasureImageJob : IJob
    {
        #region Job Settings

        public const string JobIdentity = "MeasureImageJob";
        public const string TriggerIdentity = "MeasureImageTrigger";
        public const string GroupIdentity = "MeasureImageGroup";
        public static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

        #endregion

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Executing MeasureImageJob.");

            using var dbContext = new GbmDbContext();

            foreach (var task in dbContext.Tasks.Where(t => t.Status == (int) MeasureTaskStatus.Segmented))
            {
                var args =
                    $"{ApplicationContext.Current.Configuration["PythonRepository"]}/measure.py {ApplicationContext.Current.Configuration["PythonRepository"]}/settings.json {task.OutputImageFilePath} {ApplicationContext.Current.Configuration["OutputMetricsDirectory"]}/{task.Id}.txt";

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

                using (TextReader tr = new StreamReader($"{ApplicationContext.Current.Configuration["OutputMetricsDirectory"]}/{task.Id}.txt"))
                    task.OutputMetrics = tr.ReadToEnd();

                dbContext.Tasks.Single(t => t.Id == task.Id).Status = (int)MeasureTaskStatus.Finished;
                dbContext.SaveChanges();
            }

            Console.WriteLine("MeasureImageJob finished.");

            return Task.CompletedTask;
        }
    }
}

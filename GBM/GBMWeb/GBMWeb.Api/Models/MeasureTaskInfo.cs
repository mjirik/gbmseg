using System;
using GBMWeb.Data.Models;

namespace GBMWeb.Api.Models
{
    public class MeasureTaskInfo
    {
        public Guid Id { get; set; }
        public MeasureTaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OutputMetrics { get; set; }
    }
}

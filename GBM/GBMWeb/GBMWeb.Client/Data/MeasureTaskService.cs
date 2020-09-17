using GBMWeb.Api.Models;
using System.Collections.Generic;
using GBMWeb.Shared;
using Microsoft.AspNetCore.Http;

namespace GBMWeb.Client.Data
{
    public class MeasureTaskService : BaseService
    {
        public IList<MeasureTaskInfo> GetTasks()
        {
            var url = $"{ApplicationContext.Current.Configuration["Services:MeasureTask"]}/list";
            return DoGet<IList<MeasureTaskInfo>>(url);
        }

        public void CreateTask(IFormFile file)
        {
            var url = $"{ApplicationContext.Current.Configuration["Services:MeasureTask"]}/create";
            DoPut(url, file);
        }
    }
}

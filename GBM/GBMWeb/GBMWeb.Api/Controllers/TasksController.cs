using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GBMWeb.Api.Models;
using GBMWeb.Data.Contexts;
using GBMWeb.Data.Models;
using GBMWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GBMWeb.Api.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TasksController : ControllerBase
    {
        [HttpGet("list")]
        public IList<MeasureTaskInfo> List()
        {
            using var dbContext = new GbmDbContext();
            return dbContext.Tasks.Select(x => new MeasureTaskInfo
            {
                Id = new Guid(x.Id),
                CreatedAt = x.CreatedAt,
                Status = (MeasureTaskStatus) x.Status,
                OutputMetrics = x.OutputMetrics
            }).OrderBy(x => x.CreatedAt).ToList();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromQuery] string redirect_uri, IFormFile file)
        {
            using var dbContext = new GbmDbContext();

            if (file == null)
                return BadRequest("Requires input image file");

            var id = Guid.NewGuid().ToString();

            var filePath = $"{ApplicationContext.Current.Configuration["InputImagesDirectory"]}/{id}.jpg";

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            dbContext.Tasks.Add(new MeasureTask
            {
                Id = id.ToLower(),
                CreatedAt = DateTime.Now,
                InputImageMime = file.ContentType,
                InputImageFilePath = filePath,
                Status = (int) MeasureTaskStatus.Created,
                OutputImageFilePath = $"{ApplicationContext.Current.Configuration["OutputImagesDirectory"]}/{id}.png",
                OutputImageMime = "image/png",
            });

            dbContext.SaveChanges();

            if (string.IsNullOrEmpty(redirect_uri))
                return Ok();

            return Redirect(redirect_uri);
        }

        [HttpGet("input_image")]
        public IActionResult GetInputImage(Guid id)
        {
            using var dbContext = new GbmDbContext();

            var task = dbContext.Tasks.SingleOrDefault(x => x.Id == id.ToString().ToLower());

            if (task == null)
                return NotFound();

            if (task.Status != (int) MeasureTaskStatus.Finished)
                return BadRequest("Task is not finished");

            var stream = new FileInfo(task.InputImageFilePath).OpenRead();
            return File(stream, task.InputImageMime);
        }

        [HttpGet("output_image")]
        public IActionResult GetOutputImage(Guid id)
        {
            using var dbContext = new GbmDbContext();

            var task = dbContext.Tasks.SingleOrDefault(x => x.Id == id.ToString().ToLower());

            if (task == null)
                return NotFound();

            if (task.Status != (int)MeasureTaskStatus.Finished)
                return BadRequest("Task is not finished");

            var stream = new FileInfo(task.OutputImageFilePath).OpenRead();
            return File(stream, task.OutputImageMime);
        }

        [HttpGet("delete")]
        public IActionResult Delete([FromQuery] string redirect_uri, Guid id)
        {
            using var dbContext = new GbmDbContext();

            var task = dbContext.Tasks.SingleOrDefault(x => x.Id == id.ToString().ToLower());

            if (task == null)
                return NotFound();

            dbContext.Tasks.Remove(task);
            dbContext.SaveChanges();

            if (string.IsNullOrEmpty(redirect_uri))
                return Ok();

            return Redirect(redirect_uri);
        }
    }
}

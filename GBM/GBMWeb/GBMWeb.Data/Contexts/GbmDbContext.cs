using GBMWeb.Data.Models;
using GBMWeb.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GBMWeb.Data.Contexts
{
    public class GbmDbContext : DbContext
    {
        public DbSet<MeasureTask> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(
                ApplicationContext.Current.Configuration.GetConnectionString("DefaultConnection"));
        }
    }
}

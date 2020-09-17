using Microsoft.Extensions.Configuration;

namespace GBMWeb.Shared
{
    public class ApplicationContext
    {
        public IConfiguration Configuration { get; }

        public static ApplicationContext Current { get; private set; }

        private ApplicationContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static void Create(IConfiguration configuration)
        {
            Current = new ApplicationContext(configuration);
        }
    }
}

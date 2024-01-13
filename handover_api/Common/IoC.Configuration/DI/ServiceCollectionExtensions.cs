using handover_api.Service;
using System.Reflection;

namespace handover_api.Common.IoC.Configuration.DI
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services != null)
            {
                services.AddScoped<AuthLayerService>();
            }
        }

        public static void ConfigureMappings(this IServiceCollection services)
        {
            //Automap settings
            services?.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}

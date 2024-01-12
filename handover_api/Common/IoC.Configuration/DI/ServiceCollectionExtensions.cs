using handover_api.Service;

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
    }
}

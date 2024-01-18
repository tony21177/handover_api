using handover_api.Service;
using handover_api.Utils;
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
                services.AddScoped<MemberService>();
                services.AddSingleton<JwtHelpers>();
            }
        }

        public static void ConfigureMappings(this IServiceCollection services)
        {
            //Automap settings
            services?.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}

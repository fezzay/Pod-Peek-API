using Microsoft.Extensions.DependencyInjection;
using PodPeek.Domain.Interfaces;
using PodPeek.Domain.Services;

namespace PodPeek.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKubeServices(this IServiceCollection services)
        {
            return services.AddSingleton<IKubeService, KubeService>();
        }
    }
}

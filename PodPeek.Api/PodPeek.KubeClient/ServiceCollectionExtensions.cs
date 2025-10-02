using Microsoft.Extensions.DependencyInjection;
using PodPeek.Domain.Interfaces;

namespace PodPeek.KubeClient
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKubeServices(this IServiceCollection services)
        {
            return services.AddSingleton<IKubeClient, KubeClient>();
        }
    }
}

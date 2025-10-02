using PodPeek.Domain.Models.Kubernetes;

namespace PodPeek.Domain.Interfaces
{
    public interface IKubeClient
    {
        Task<IEnumerable<Pod>> GetPodsAsync(string namespaceName, IEnumerable<string>? serviceNames);
        Task<IEnumerable<Ingress>> GetIngressesAsync(string namespaceName);
        Task<IEnumerable<Service>> GetServicesAsync(string namespaceName);
    }
}

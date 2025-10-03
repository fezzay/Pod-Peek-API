using k8s;
using PodPeek.Domain.Interfaces;
using PodPeek.Domain.Models.Kubernetes;

namespace PodPeek.KubeClient
{
    public class KubeClient : IKubeClient
    {
        private Kubernetes _client => new Kubernetes(_config);
        private KubernetesClientConfiguration _config => KubernetesClientConfiguration.InClusterConfig();

        public async Task<IEnumerable<Pod>> GetPodsAsync(string namespaceName, IEnumerable<string>? serviceNames = null)
        {
            var podList = await _client.ListNamespacedPodAsync(namespaceName);
            if (serviceNames == null)
            {
                serviceNames = (await GetServicesAsync(namespaceName)).Select(s => s.Name);
            }

            var tasks = podList.Items.Select(p => Task.Run(() => PodMapper.Map(p, serviceNames)));
            var pods = await Task.WhenAll(tasks);

            return pods;
        }

        public async Task<IEnumerable<Ingress>> GetIngressesAsync(string namespaceName)
        {
            var ingressList = await _client.ListNamespacedIngressAsync(namespaceName);
            return ingressList.Items.Select(IngressMapper.Map);
        }

        public async Task<IEnumerable<Service>> GetServicesAsync(string namespaceName)
        {
            var serviceList = await _client.ListNamespacedServiceAsync(namespaceName);
            return serviceList.Items.Select(ServiceMapper.Map);
        }
    }
}

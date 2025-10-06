using k8s;
using PodPeek.Domain.Interfaces;
using PodPeek.Domain.Models.Kubernetes;

namespace PodPeek.KubeClient
{
    public class KubeClient : IKubeClient
    {
        private Kubernetes _client => new Kubernetes(_config);
        private KubernetesClientConfiguration _config => KubernetesClientConfiguration.InClusterConfig();

        public async Task<IEnumerable<Pod>> GetPodsAsync(string namespaceName, IEnumerable<string>? serviceNames = null, IEnumerable<string>? ingressHosts = null)
        {
            var podList = await _client.ListNamespacedPodAsync(namespaceName);

            if (serviceNames == null)
            {
                var services = await GetServicesAsync(namespaceName) ?? Enumerable.Empty<Service>();
                serviceNames = services.Select(s => s.Name);
            }

            if (ingressHosts == null)
            {
                var ingresses = await GetIngressesAsync(namespaceName) ?? Enumerable.Empty<Ingress>();
                ingressHosts = ingresses.SelectMany(i => i.Hosts);
            }

            serviceNames ??= Enumerable.Empty<string>();
            ingressHosts ??= Enumerable.Empty<string>();

            if (!podList.Items.Any())
            {
                return Enumerable.Empty<Pod>();
            }

            var tasks = podList.Items.Select(p => Task.Run(() => PodMapper.Map(p, serviceNames, ingressHosts)));
            var pods = await Task.WhenAll(tasks);

            return pods;
        }

        public async Task<IEnumerable<Ingress>> GetIngressesAsync(string namespaceName)
        {
            var ingressList = await _client.ListNamespacedIngressAsync(namespaceName);

            if (ingressList.Items == null || !ingressList.Items.Any())
            {
                return Enumerable.Empty<Ingress>();
            }

            return ingressList.Items.Select(IngressMapper.Map);
        }

        public async Task<IEnumerable<Service>> GetServicesAsync(string namespaceName)
        {
            var serviceList = await _client.ListNamespacedServiceAsync(namespaceName);

            if (serviceList.Items == null || !serviceList.Items.Any())
            {
                return Enumerable.Empty<Service>();
            }

            return serviceList.Items.Select(ServiceMapper.Map);
        }
    }
}

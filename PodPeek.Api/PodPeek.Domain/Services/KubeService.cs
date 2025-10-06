using PodPeek.Domain.Interfaces;
using PodPeek.Domain.Models.Graph;
using System.Linq;

namespace PodPeek.Domain.Services
{
    public class KubeService(IKubeClient _client) : IKubeService
    {
        public async Task<Graph> BuildGraph(string namespaceName)
        {
            var ingresses = (await _client.GetIngressesAsync(namespaceName)).ToList();
            var services = (await _client.GetServicesAsync(namespaceName)).ToList();
            var pods = (await _client.GetPodsAsync(namespaceName, services.Select(s => s.Name))).ToList();

            var nodes = new List<Node>();
            var edges = new List<Edge>();

            // ---- Phase 1: Add nodes ----

            // Pod nodes
            int podY = 0;
            foreach (var pod in pods)
            {
                nodes.Add(new Node
                {
                    Id = Guid.NewGuid(),
                    Type = "pod",
                    Data = pod,
                    Position = new Position { X = 0, Y = podY * 150 }
                });
                podY++;
            }

            // Service nodes
            int svcY = 0;
            foreach (var svc in services)
            {
                nodes.Add(new Node
                {
                    Id = Guid.NewGuid(),
                    Type = "service",
                    Data = svc,
                    Position = new Position { X = 500, Y = svcY * 150 }
                });
                svcY++;
            }

            // Ingress nodes
            int ingY = 0;
            foreach (var ingress in ingresses)
            {
                nodes.Add(new Node
                {
                    Id = Guid.NewGuid(),
                    Type = "ingress",
                    Data = ingress,
                    Position = new Position { X = 1000, Y = ingY * 150 }
                });
                ingY++;
            }

            // ---- Phase 2: Build edges ----

            // Pod -> Service
            foreach (var pod in pods)
            {
                var podNode = nodes.FirstOrDefault(n => n.Data == pod);
                if (podNode == null) continue;

                foreach (var container in pod.Containers)
                {
                    foreach (var port in container.Ports)
                    {
                        var svc = services.FirstOrDefault(s =>
                            s.Ports.Any(sp => sp.InternalPort == port.InternalPort) &&
                            s.Selector.Any(kv => pod.Selector.TryGetValue(kv.Key, out var val) && val == kv.Value));

                        if (svc != null)
                        {
                            var svcNode = nodes.FirstOrDefault(n => n.Data == svc);
                            if (svcNode == null) continue;

                            edges.Add(new Edge
                            {
                                Source = podNode.Id,
                                Target = svcNode.Id,
                                SourceHandle = $"{container.Name}-port-{port.InternalPort}",
                                TargetHandle = $"port-{port.InternalPort}"
                            });
                        }
                    }
                }
            }

            // Service OR ingress -> Pod (via env vars)
            foreach (var pod in pods)
            {
                var podNode = nodes.FirstOrDefault(n => n.Data == pod);
                if (podNode == null) continue;

                foreach (var container in pod.Containers)
                {
                    foreach (var env in container.EnvironmentVariables)
                    {
                        var svc = services.FirstOrDefault(s =>
                            env.Value.Contains(s.Name, StringComparison.OrdinalIgnoreCase) &&
                            s.Ports.Any(p => p.TargetPort.HasValue && env.Value.Contains(p.TargetPort.Value.ToString()))
                        );

                        if (svc != null)
                        {
                            var svcNode = nodes.FirstOrDefault(n => n.Data == svc);
                            if (svcNode == null) continue;

                            var matchedPort = svc.Ports.FirstOrDefault(p =>
                                p.TargetPort.HasValue && env.Value.Contains(p.TargetPort.Value.ToString()));

                            if (matchedPort != null)
                            {
                                edges.Add(new Edge
                                {
                                    Source = svcNode.Id,
                                    Target = podNode.Id,
                                    SourceHandle = $"targetPort-{matchedPort.TargetPort}",
                                    TargetHandle = $"{container.Name}-env-{env.Key}"
                                });
                            }
                        }

                        var ing = ingresses.FirstOrDefault(i =>
                            i.Hosts.Any(h =>
                                env.Value.Contains(h, StringComparison.OrdinalIgnoreCase)));

                        if (ing != null)
                        {
                            var ingNode = nodes.FirstOrDefault(n => n.Data == ing);
                            if (ingNode == null) continue;

                            var matchedHost = ing.Hosts.FirstOrDefault(h => env.Value.Contains(h, StringComparison.OrdinalIgnoreCase));

                            edges.Add(new Edge
                            {
                                Source = ingNode.Id,
                                Target = podNode.Id,
                                SourceHandle = $"ingress-{matchedHost}",
                                TargetHandle = $"{container.Name}-env-{env.Key}"
                            });
                        }
                    }
                }
            }

            // Ingress -> Service
            foreach (var ingress in ingresses)
            {
                var ingNode = nodes.FirstOrDefault(n => n.Data == ingress);
                if (ingNode == null) continue;

                foreach (var rule in ingress.Rules)
                {
                    var svc = services.FirstOrDefault(s => s.Name == rule.Service);
                    if (svc != null)
                    {
                        var svcNode = nodes.FirstOrDefault(n => n.Data == svc);
                        if (svcNode == null) continue;

                        edges.Add(new Edge
                        {
                            Id = Guid.NewGuid(),
                            Source = svcNode.Id,
                            Target = ingNode.Id,
                            SourceHandle = $"targetPort-{rule.Port}",
                            TargetHandle = $"rule-{rule.Port}"
                        });
                    }
                }
            }

            return new Graph
            {
                Nodes = nodes,
                Edges = edges
            };
        }
    }
}

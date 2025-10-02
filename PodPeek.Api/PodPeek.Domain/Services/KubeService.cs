using PodPeek.Domain.Interfaces;
using PodPeek.Domain.Models.Graph;
using PodPeek.Domain.Models.Kubernetes;
using System.Linq;

namespace PodPeek.Domain.Services
{
    public class KubeService(IKubeClient _client) : IKubeService
    {
        public async Task<Graph> BuildGraph(string namespaceName)
        {
            var ingresses = await _client.GetIngressesAsync(namespaceName);
            var services = await _client.GetServicesAsync(namespaceName);
            var pods = await _client.GetPodsAsync(namespaceName, services.Select(s => s.Name));

            var nodes = new List<Node>();
            var edges = new List<Edge>();

            // 1. Pod nodes
            foreach (var pod in pods)
            {
                var YAxis = 0;
                nodes.Add(new Node
                {
                    Id = Guid.NewGuid(),
                    Type = "pod",
                    Data = pod,
                    Position = new Position { X = 0, Y = YAxis * 500 }
                });
                YAxis++;
            }

            // 2. Service nodes
            foreach (var svc in services)
            {
                var YAxis = 0;
                nodes.Add(new Node
                {
                    Id = Guid.NewGuid(),
                    Type = "service",
                    Data = svc,
                    Position = new Position { X = 500, Y = YAxis * 500 }
                });
                YAxis++;
            }

            // 3. Ingress nodes
            foreach (var ingress in ingresses)
            {
                var YAxis = 0;
                nodes.Add(new Node
                {
                    Id = Guid.NewGuid(),
                    Type = "ingress",
                    Data = ingress,
                    Position = new Position { X = 1000, Y = YAxis * 500 }
                });
                YAxis++;
            }

            // ---- Phase 2: Build Edges ----

            // Pod -> Service
            foreach (var pod in pods)
            {
                var podNode = nodes.First(n => n.Data == pod);

                foreach (var container in pod.Containers)
                {
                    foreach (var port in container.Ports)
                    {
                        var svc = services.FirstOrDefault(s =>
                            s.Ports.Any(sp => sp.InternalPort == port.InternalPort) &&
                            s.Selector.Any(kv => pod.Selector.TryGetValue(kv.Key, out var val) && val == kv.Value));

                        if (svc != null)
                        {
                            var svcNode = nodes.First(n => n.Data == svc);

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

            // Service -> Pod (via env vars)
            foreach (var pod in pods)
            {
                var podNode = nodes.First(n => n.Data == pod);

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
                            var svcNode = nodes.First(n => n.Data == svc);

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
                    }
                }
            }

            foreach (var ingress in ingresses)
            {
                var ingNode = nodes.First(n => n.Data == ingress);

                foreach (var rule in ingress.Rules)
                {
                    var svc = services.FirstOrDefault(s => s.Name == rule.Service);
                    if (svc != null)
                    {
                        var svcNode = nodes.First(n => n.Data == svc);

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

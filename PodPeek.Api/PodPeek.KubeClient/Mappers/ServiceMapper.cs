using k8s.Models;
using PodPeek.Domain.Models.Kubernetes;

public static class ServiceMapper
{
    public static Service Map(V1Service v1Service)
    {
        var ports = v1Service.Spec?.Ports?.Select(p =>
        {
            int? targetPort = null;

            if (p.TargetPort != null)
            {
                if (int.TryParse(p.TargetPort.Value, out int port))
                    targetPort = port;
            }

            return new Port
            {
                Name = p.Name,
                InternalPort = p.Port,
                TargetPort = targetPort
            };
        }) ?? Enumerable.Empty<Port>();

        return new Service
        {
            Name = v1Service.Metadata?.Name ?? string.Empty,
            Type = v1Service.Spec?.Type ?? "ClusterIP",
            Ports = ports,
            Selector = v1Service.Spec?.Selector ?? new Dictionary<string, string>()
        };
    }
}

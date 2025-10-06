using k8s.Models;
using PodPeek.Domain.Models.Kubernetes;

public static class PodMapper
{
    public static Pod Map(V1Pod v1Pod, IEnumerable<string> servicesInNamespace, IEnumerable<string> ingressInNamespace)
    {
        var containers = v1Pod.Spec?.Containers
            .Select(c => MapContainer(c, servicesInNamespace, ingressInNamespace))
            ?? Enumerable.Empty<Container>();

        var status = v1Pod.Status?.Phase ?? "Unknown";
        int running = v1Pod.Status?.ContainerStatuses?.Count(cs => cs?.Ready == true) ?? 0;
        int total = v1Pod.Spec?.Containers.Count ?? 0;

        return new Pod
        {
            Name = v1Pod.Metadata?.Name ?? string.Empty,
            Status = status,
            ReplicasRunning = running,
            ReplicasCompleted = total,
            Containers = containers,
            // Map pod labels to AppSelector
            Selector = v1Pod.Metadata?.Labels ?? new Dictionary<string, string>()
        };
    }

    private static Container MapContainer(V1Container v1Container, IEnumerable<string> servicesInNamespace, IEnumerable<string> ingressInNamespace)
    {
        var ports = v1Container.Ports?.Select(p => new Port
        {
            Name = p.Name,
            InternalPort = p.ContainerPort
        }) ?? Enumerable.Empty<Port>();

        var mounts = v1Container.VolumeMounts?.Select(vm => vm.MountPath) ?? Enumerable.Empty<string>();

        var envVars = v1Container.Env?
            .Where(e => !string.IsNullOrEmpty(e.Value) && 
            (servicesInNamespace.Any(s => e.Value.Contains(s)) || ingressInNamespace.Any(i => e.Value.Contains(i))))
            .ToDictionary(e => e.Name, e => e.Value)
            ?? new Dictionary<string, string>();

        return new Container
        {
            Name = v1Container.Name,
            Image = v1Container.Image,
            Ports = ports,
            Mounts = mounts,
            EnvironmentVariables = envVars
        };
    }
}

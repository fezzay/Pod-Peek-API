using k8s.Models;
using PodPeek.Domain.Models.Kubernetes;

public static class IngressMapper
{
    public static Ingress Map(V1Ingress v1Ingress)
    {
        var rules = v1Ingress.Spec?.Rules?
            .SelectMany(r => r.Http?.Paths.Select(p => new Rule
            {
                Path = p.Path ?? string.Empty,
                Service = p.Backend?.Service?.Name ?? string.Empty,
                Port = p.Backend?.Service?.Port?.Number ?? 0
            }) ?? Enumerable.Empty<Rule>())
            ?? Enumerable.Empty<Rule>();

        var hosts = v1Ingress.Spec?.Rules?.Select(r => r.Host ?? string.Empty) ?? Enumerable.Empty<string>();

        return new Ingress
        {
            Name = v1Ingress.Metadata?.Name ?? string.Empty,
            Hosts = hosts,
            Rules = rules
        };
    }
}

namespace PodPeek.Domain.Models.Kubernetes
{
    public class Service : KubeObject
    {
        public string Type { get; set; } = string.Empty;
        public IEnumerable<Port> Ports { get; set; } = [];
    }
}

using PodPeek.Domain.Models.Kubernetes;

namespace PodPeek.Domain.Models.Graph
{
    public class Node
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;
        public KubeObject Data { get; set; } = new();
        public Position Position { get; set; } = new();
    }
}

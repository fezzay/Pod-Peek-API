using PodPeek.Domain.Models.Kubernetes;

namespace PodPeek.Domain.Models.Graph
{
    public class Node
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; }
        public KubeObject Data { get; set; }
        public Position Position { get; set; }
    }
}

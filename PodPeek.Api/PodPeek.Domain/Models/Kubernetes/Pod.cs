namespace PodPeek.Domain.Models.Kubernetes
{
    public class Pod : KubeObject
    {
        public string Status { get; set; } = string.Empty;
        public int ReplicasRunning { get; set; }
        public int ReplicasCompleted { get; set; }
        public IEnumerable<Container> Containers { get; set; } = [];
    }
}

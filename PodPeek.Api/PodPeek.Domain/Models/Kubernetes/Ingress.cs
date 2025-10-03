namespace PodPeek.Domain.Models.Kubernetes
{
    public class Ingress : KubeObject
    {
        public IEnumerable<string> Hosts { get; set; } = [];
        public IEnumerable<Rule> Rules { get; set; } = [];
    }
}

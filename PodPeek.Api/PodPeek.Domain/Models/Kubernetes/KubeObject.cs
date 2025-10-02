namespace PodPeek.Domain.Models.Kubernetes
{
    public class KubeObject
    {
        public string Name { get; set; }
        public IDictionary<string, string> Selector { get; set; }
    }
}

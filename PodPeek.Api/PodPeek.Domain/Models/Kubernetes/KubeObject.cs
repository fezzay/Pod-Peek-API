namespace PodPeek.Domain.Models.Kubernetes
{
    public class KubeObject
    {
        public string Name { get; set; } = string.Empty;
        public IDictionary<string, string> Selector { get; set; } = new Dictionary<string, string>();
    }
}

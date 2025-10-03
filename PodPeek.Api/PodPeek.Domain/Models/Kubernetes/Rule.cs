namespace PodPeek.Domain.Models.Kubernetes
{
    public class Rule
    {
        public string Path { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}

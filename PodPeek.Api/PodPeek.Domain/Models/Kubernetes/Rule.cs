namespace PodPeek.Domain.Models.Kubernetes
{
    public class Rule
    {
        public string Path { get; set; }
        public string Service { get; set; }
        public int Port { get; set; }
    }
}

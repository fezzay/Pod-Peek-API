namespace PodPeek.Domain.Models.Kubernetes
{
    public class Port
    {
        public string Name { get; set; } = string.Empty;
        public int InternalPort { get; set; }
        public int? TargetPort { get; set; }
    }
}

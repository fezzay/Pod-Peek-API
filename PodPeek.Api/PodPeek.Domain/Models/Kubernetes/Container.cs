namespace PodPeek.Domain.Models.Kubernetes
{
    public class Container
    {
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public IEnumerable<Port> Ports { get; set; } = [];
        public IEnumerable<string> Mounts { get; set; } = [];
        public Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();
    }
}

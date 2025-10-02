namespace PodPeek.Domain.Models.Kubernetes
{
    public class Container
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public IEnumerable<Port> Ports { get; set; }
        public IEnumerable<string> Mounts { get; set; }
        public Dictionary<string, string> EnvironmentVariables { get; set; }
    }
}

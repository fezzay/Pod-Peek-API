namespace PodPeek.Domain.Models.Graph
{
    public class Edge
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid Source { get; set; }
        public Guid Target { get; set; }
        public string SourceHandle { get; set; } = string.Empty;
        public string TargetHandle { get; set; } = string.Empty;
    }
}

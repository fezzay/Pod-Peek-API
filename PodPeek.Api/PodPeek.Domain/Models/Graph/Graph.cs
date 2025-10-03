namespace PodPeek.Domain.Models.Graph
{
    public class Graph
    {
        public IEnumerable<Node> Nodes { get; set; } = [];
        public IEnumerable<Edge> Edges { get; set; } = [];
    }
}

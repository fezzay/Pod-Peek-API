using PodPeek.Domain.Models.Graph;

namespace PodPeek.Domain.Interfaces
{
    public interface IKubeService
    {
        Task<Graph> BuildGraph(string namespaceName);
    }
}

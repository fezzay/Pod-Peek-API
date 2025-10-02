using Microsoft.AspNetCore.Mvc;
using PodPeek.Domain.Interfaces;
using PodPeek.Domain.Models.Graph;

namespace PodPeek.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GraphController(IKubeService _kubeService, ILogger<GraphController> _logger) : ControllerBase
    {
        [HttpGet("{namespaceName}", Name = "Node")]
        public async Task<Graph> GetNodeGraph([FromRoute] string namespaceName)
        {
            return await _kubeService.BuildGraph(namespaceName);
        }
    }
}

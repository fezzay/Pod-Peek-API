using System.Text.Json.Serialization;

namespace PodPeek.Domain.Models.Graph
{
    public class Position
    {
        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }
    }
}

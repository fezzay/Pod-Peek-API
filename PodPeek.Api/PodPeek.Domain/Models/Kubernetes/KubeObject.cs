namespace PodPeek.Domain.Models.Kubernetes
{
    using System.Text.Json.Serialization;

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kubeType")]
    [JsonDerivedType(typeof(Pod), "pod")]
    [JsonDerivedType(typeof(Service), "service")]
    [JsonDerivedType(typeof(Ingress), "ingress")]
    public class KubeObject
    {
        public string Name { get; set; } = string.Empty;
        public IDictionary<string, string> Selector { get; set; } = new Dictionary<string, string>();
    }
}

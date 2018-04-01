using Newtonsoft.Json;

namespace WaultBlock.Identities.DataObjects
{
    public class ClaimDefinitionObjectData
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "attr_names")]
        public string[] AttributeNames { get; set; }
    }
}

using Newtonsoft.Json;

namespace WaultBlock.Identities.DataObjects
{
    public class ClaimDefinitionObject
    {
        [JsonProperty(PropertyName = "seqNo")]
        public int SequenceNumber { get; set; }

        [JsonProperty(PropertyName = "data")]
        public ClaimDefinitionObjectData Data { get; set; }
    }
}

using Newtonsoft.Json;

namespace WaultBlock.Identities.DataObjects
{
    public class DidJson
    {
        [JsonProperty(PropertyName = "seed")]
        public string Seed { get; private set; }

        public DidJson(string seed)
        {
            const int requiredLength = 32;

            if (seed.Length > requiredLength)
            {
                seed = seed.Substring(0, requiredLength);
            }

            while (seed.Length < requiredLength)
            {
                seed = "0" + seed;
            }

            Seed = seed;
        }

        public static string GetJson(string seed)
        {
            var d = new DidJson(seed);
            return JsonConvert.SerializeObject(d);
        }
    }
}

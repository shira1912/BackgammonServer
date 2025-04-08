using Newtonsoft.Json;
using System;

namespace DataProtocols
{
    [Serializable]
    public class RSAPublicKey : Data
    {
        [JsonProperty]
        public string PublicKey;
    }
}

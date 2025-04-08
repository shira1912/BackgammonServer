using Newtonsoft.Json;
using System;

namespace DataProtocols
{
    [Serializable]
    public class AESKey : Data
    {
        [JsonProperty]
        public string IV;

        [JsonProperty]
        public string Key;
    }
}

using Newtonsoft.Json;
using System;

namespace DataProtocols
{
    [Serializable]
    public class Data
    {
        [JsonProperty]
        public DataType DataType;
    }
}

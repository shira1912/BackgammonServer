using Newtonsoft.Json;
using System;

namespace DataProtocols
{
    public static class ConvertUtils
    {
        public static DataType GetDataType(string input) => JsonConvert.DeserializeObject<Data>(input).DataType;

        public static T Deserialize<T>(string input)
        {
            var data = JsonConvert.DeserializeObject<Data>(input);

            switch (data.DataType)
            {
                case DataType.None:
                    throw new Exception("Invalid data type");

                case DataType.AESKey:
                    if (typeof(T) == typeof(AESKey))
                    {
                        return (T)(object)JsonConvert.DeserializeObject<AESKey>(input);
                    }
                    break;

                case DataType.RSAKey:
                    if (typeof(T) == typeof(RSAPublicKey))
                    {
                        return (T)(object)JsonConvert.DeserializeObject<RSAPublicKey>(input);
                    }
                    break;
                default:
                    throw new Exception("Unknown data type");
            }

            throw new InvalidOperationException("Invalid cast or unsupported data type.");
        }
    }
}

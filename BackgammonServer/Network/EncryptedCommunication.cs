using DataProtocols;
using Encryption;
using Newtonsoft.Json;

namespace BackgammonServer.Network
{
    public class EncryptedCommunication
    {
        public event Action<string, string> OnMessageReceive;
        public event Action<string> OnClientAdd;

        private NetworkManager m_NetworkManager;
        private Dictionary<string, ClientData> m_Clients;

        public void Connect()
        {
            m_NetworkManager = new NetworkManager();
            m_Clients = new Dictionary<string, ClientData>();

            m_NetworkManager.OnMessageReceive += OnMessageReceiveFromClient;

            m_NetworkManager.Connect();
        }

        public void Broadcast(string message)
        {
            string clientIp = "";

            foreach (var client in m_Clients)
            {
                clientIp = client.Key;
                SendMessage(message, clientIp);
            }
        }

        public void SendMessage(string message, string clientIp)
        {
            var chirpedMessage = m_Clients[clientIp].AESEncryption.Encrypt(message);
            m_NetworkManager.SendMessage(chirpedMessage, clientIp);
        }

        public void RemoveAllTheClients()
        {
            m_Clients.Clear();
            m_NetworkManager.RemoveAllTheClients();
        }

        public void Disconnect()
        {
            m_NetworkManager.Disconnect();
        }

        private void OnMessageReceiveFromClient(string message, string ip)
        {
            if (m_Clients.ContainsKey(ip))
            {
                var decipheredMessage = m_Clients[ip].AESEncryption.Decrypt(message);
                OnMessageReceive.Invoke(decipheredMessage, ip);
            }
            else
            {
                AddNewClient(message, ip);
            }
        }

        private void AddNewClient(string key, string ip)
        {
            var clientData = new ClientData();
            clientData.AESEncryption = new AESEncryption();
            clientData.RSAEncryption = new RSAEncryption();

            m_Clients.Add(ip, clientData);

            var publicKey = Deserialize<RSAPublicKey>(key);

            clientData.RSAEncryption.LoadPublicKey(publicKey.PublicKey);
            clientData.AESEncryption.GenerateKey();

            var aesKey = new AESKey();
            aesKey.DataType = DataType.AESKey;
            aesKey.Key = clientData.AESEncryption.GetKey();
            aesKey.IV = clientData.AESEncryption.GetIV();

            var message = JsonConvert.SerializeObject(aesKey, Newtonsoft.Json.Formatting.Indented);

            var chipperedMessage = clientData.RSAEncryption.Encrypt(message);

            m_NetworkManager.SendMessage(chipperedMessage, ip);

            OnClientAdd?.Invoke(ip);
        }

        private T Deserialize<T>(string input)
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

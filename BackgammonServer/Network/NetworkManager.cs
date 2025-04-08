using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackgammonServer.Network
{
    public class NetworkManager
    {
        public event Action<string> OnClientRemove;
        public event Action<string> OnClientAdd;
        public event Action<string, string> OnMessageReceive;

        private const int k_PortNo = 5000;
        private const string k_IpAddress = "127.0.0.1";
        private const int k_NumberOfUserConnected = 2;

        private List<ChatClient> m_AllClients = new List<ChatClient>();
        private TcpListener m_Listener;

        public NetworkManager() { }

        public void Connect()
        {
            System.Net.IPAddress localAdd = System.Net.IPAddress.Parse(k_IpAddress);

            m_Listener = new TcpListener(localAdd, k_PortNo);

            Console.WriteLine("Simple TCP Server");
            Console.WriteLine("Listening to ip {0} port: {1}", k_IpAddress, k_PortNo);
            Console.WriteLine("Server is ready.");

            m_Listener.Start();

            Thread thread = new Thread(new ThreadStart(Listen));
            thread.Start();

            Console.WriteLine("Network manager has been instantiated");
        }

        private void Listen()
        {
            while (true)
            {  
                AddNewClient(m_Listener);
            }
        }

        public void Broadcast(string str)
        {
            foreach (var item in m_AllClients)
            {
                item.SendMessage(str);
            }
        }

        public void SendMessage(string message, string clientIp)
        {
            foreach (var client in m_AllClients)
            {
                if (client.GetClientIP == clientIp)
                {
                    client.SendMessage(message);
                }
            }
        }

        public void RemoveAllTheClients() => m_AllClients.Clear();

        public void Disconnect() => m_Listener?.Stop();

        private void AddNewClient(TcpListener listener)
        {
            TcpClient tcpClient = listener.AcceptTcpClient();
            lock (m_AllClients)
            {
                Console.WriteLine("New socket: " + tcpClient.Client.RemoteEndPoint.ToString());
                ChatClient user = new ChatClient(tcpClient);
                user.OnReceiveDataFromTheClient += OnMesageReceiveFromClient;
                user.OnRemove += OnRemove;
                OnClientAdd?.Invoke(user.GetClientIP);
                m_AllClients.Add(user);
            }
        }

        private void OnRemove(ChatClient chatClient, string id)
        {
            m_AllClients.Remove(chatClient);
            OnClientRemove?.Invoke(id);
        }

        private void OnMesageReceiveFromClient(string message, string ip)
        {
            OnMessageReceive?.Invoke(message, ip);

            if (OnMessageReceive == null)
            {
                Console.WriteLine("no one has yet subscribed to this event");
            }
        }
    }
}

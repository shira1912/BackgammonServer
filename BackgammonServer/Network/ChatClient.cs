using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonServer.Network
{

    public class ChatClient
    {
        public event Action<string, string> OnReceiveDataFromTheClient;
        public event Action<ChatClient, string> OnRemove;

        private TcpClient _client;
        private string _clientIP;

        private byte[] data;

        public string GetClientIP
        {
            get { return _clientIP; }
        }

        public ChatClient(TcpClient client)
        {
            _client = client;
            _clientIP = client.Client.RemoteEndPoint.ToString();
            data = new byte[_client.ReceiveBufferSize];

            _client.GetStream().BeginRead(data, 0, Convert.ToInt32(_client.ReceiveBufferSize),
                ReceiveMessage, null);
        }

        public void SendMessage(string message)
        {
            try
            {
                NetworkStream ns;
                lock (_client.GetStream())
                {
                    ns = _client.GetStream();
                }

                byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
                ns.Write(bytesToSend, 0, bytesToSend.Length);
                ns.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ReceiveMessage(IAsyncResult ar)
        {
            int bytesRead;
            try
            {
                lock (_client.GetStream())
                {
                    bytesRead = _client.GetStream().EndRead(ar);
                }
                if (bytesRead < 1)
                {
                    OnRemove.Invoke(this, _clientIP);
                    return;
                }
                else
                {
                    string messageReceived = Encoding.ASCII.GetString(data, 0, bytesRead);
                    OnReceiveDataFromTheClient?.Invoke(messageReceived, _clientIP);
                }
                lock (_client.GetStream())
                {
                    _client.GetStream().BeginRead(data, 0, Convert.ToInt32(_client.ReceiveBufferSize),
                        ReceiveMessage, null);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }
    }
}


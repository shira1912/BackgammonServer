using BackgammonServer.Network;

namespace BackgammonServer.Managers
{
    internal class GameManager
    {
        private EncryptedCommunication _encryptedCommunication;
        private List<string> playersIp;
        public GameManager(EncryptedCommunication encryptedCommunication,List<string> playersIp)
        {
            _encryptedCommunication = encryptedCommunication;
            this.playersIp = playersIp;
            _encryptedCommunication.SendMessage("StartGame,1", playersIp[0]);
            _encryptedCommunication.SendMessage("StartGame,2", playersIp[1]);
        }

        public void SwitchTurn(string currentPlayer)
        {
            string nextPlayer = "1";
            if (currentPlayer == "1")
            {
                nextPlayer = "2";
            }
            BroadcastToRoom("Turn," + nextPlayer);
        }

        public void BroadcastToRoom(string message)
        {
            string clientIp = "";

            foreach (string ip in playersIp)
            {
                _encryptedCommunication.SendMessage(message, ip);
            }
        }
    }
}

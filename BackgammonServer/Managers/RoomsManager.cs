using BackgammonServer.DataBase;
using BackgammonServer.Network;

namespace BackgammonServer.Managers
{
    internal class RoomsManager
    {
        private EncryptedCommunication m_SecureNetworkManager;
        private ConnectionToUserDataBase connectionToDatabase; 
        private List<Room> m_Rooms;
        private int m_NumberOfWaitingPlayers = 0;

        private Queue<string> m_PlayersWaiting;
        private GameManager gameManager;

        public RoomsManager(EncryptedCommunication secureNetworkManager)
        {
            m_SecureNetworkManager = secureNetworkManager;
            secureNetworkManager.OnMessageReceive += ProcessClientMessage;
            m_PlayersWaiting = new Queue<string>();
        }

        private void ProcessClientMessage(string message, string ip)
        {
            string[] splitMessage = message.Split(',');


            switch (splitMessage[0])
            {
                case "InSearchForGame":
                    {
                        if (m_NumberOfWaitingPlayers == 0)
                        {
                            m_NumberOfWaitingPlayers++;
                            m_PlayersWaiting.Enqueue(ip);
                            m_SecureNetworkManager.SendMessage("Wait,", ip);
                        }
                        else if (m_NumberOfWaitingPlayers == 1)
                        {
                            m_PlayersWaiting.Enqueue(ip);
                            CreateRoom();
                            m_NumberOfWaitingPlayers = 0;
                        }
                        break;
                    }


                case "SwitchTurn":
                    {
                        {
                            gameManager.SwitchTurn(splitMessage[1]);
                        }
                        break;

                    }

                case "State":
                    {
                        gameManager.BroadcastToRoom(message);
                        break;
                    }

                case "Dice":
                    {
                        gameManager.BroadcastToRoom(message);
                        break;
                    }
                case "Win":
                    {
                        gameManager.BroadcastToRoom(message);
                        break;
                    }
                case "ResetPassword":
                    {
                        connectionToDatabase.resetPasswordByEmail(splitMessage[1], splitMessage[2]);
                        m_SecureNetworkManager.SendMessage("ResetPassword, successful", ip);
                        break;
                    }
                case "IsEmailExists":
                {
                        if (connectionToDatabase.IsEmailExist(splitMessage[1]))
                        {
                            m_SecureNetworkManager.SendMessage("IsEmailExists, true", ip);
                        }
                        break;
                }

            }
        }
        private void CreateRoom()
        {
            var players = new List<string>();

            players.Add(m_PlayersWaiting.Dequeue());
            players.Add(m_PlayersWaiting.Dequeue());

            gameManager = new GameManager(m_SecureNetworkManager, players);
        }
    }
}
using BackgammonServer.DataBase;
using BackgammonServer.Network;

namespace BackgammonServer.Managers
{
    internal class RoomsManager
    {
        private EncryptedCommunication m_SecureNetworkManager;
        private ConnectionToUserDataBase _db;

        private ConnectionToUserDataBase connectionToDatabase; 
        private static List<Room> m_Rooms = new List<Room>();
        private static List<GameManager> games = new List<GameManager>();
        private int m_NumberOfWaitingPlayers = 0;

        private Queue<string> m_PlayersWaiting;
        private Dictionary<string, int> m_Clients = new Dictionary<string, int>();
        private int roomId = 0;
        private GameManager gameManager;


        public RoomsManager(ConnectionToUserDataBase db, EncryptedCommunication secureNetworkManager)
        {
            m_SecureNetworkManager = secureNetworkManager;
            _db = db;
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
                            string nextPlayer = "1";
                            if (splitMessage[1] == "1")
                            {
                                nextPlayer = "2";
                            }
                            games[m_Clients[ip]].BroadcastToRoom("Turn," + nextPlayer);
                        }
                        break;

                    }

                case "State":
                    {
                        games[m_Clients[ip]].BroadcastToRoom(message);
                        break;
                    }

                case "Dice":
                    {
                        games[m_Clients[ip]].BroadcastToRoom(message);
                        break;
                    }
                case "Win":
                    {
                        int roomID = m_Clients[ip];
                        games[roomID].BroadcastToRoom(message);
                        removePlayer(roomID);
                        games.Remove(games[roomID]);
                        break;
                    }
                case "ResetPassword":
                    {
                        _db.resetPasswordByEmail(splitMessage[1], splitMessage[2]);
                        m_SecureNetworkManager.SendMessage("ResetPassword,successful", ip);
                        break;
                    }
                case "IsEmailExists":
                {
                        if (_db.IsEmailExist(splitMessage[1]))
                        {
                            m_SecureNetworkManager.SendMessage("IsEmailExists,true", ip);
                        }
                        else
                        {
                            m_SecureNetworkManager.SendMessage("IsEmailExists,false", ip);

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

            var room = new Room(players[0], players[1]);
            m_Rooms.Add(room);

            roomId = m_Rooms.Count - 1;
            m_Clients.Add(players[0], roomId);
            m_Clients.Add(players[1], roomId);

            games.Add(new GameManager(m_SecureNetworkManager, players));

        }

        private void removePlayer(int roomId)
        {
            foreach (string client in m_Clients.Keys)
            {
                if (m_Clients[client] == roomId)
                {
                    m_Clients.Remove(client);
                }
            }
        }
    }
}
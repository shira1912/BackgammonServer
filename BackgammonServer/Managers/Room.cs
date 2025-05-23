namespace BackgammonServer.Managers
{
    internal class Room
    {
        public string PlayerOneIp;
        public string PlayerTwoIp;

        public Room(string playerOneIp, string playerTwoIp)
        {
            PlayerOneIp = playerOneIp;
            PlayerTwoIp = playerTwoIp;
        }
    }
}

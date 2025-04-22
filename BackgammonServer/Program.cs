using BackgammonServer.DataBase;
using BackgammonServer.Managers;
using BackgammonServer.Network;

namespace BackgammonServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connection = new ConnectionToUserDataBase();
            var communication = new EncryptedCommunication();
            communication.Connect();

            var logInManager = new LogInSignUpManager(connection, communication);
            var roomsManager = new RoomsManager(connection, communication);
        }
    }
}

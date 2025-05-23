using BackgammonServer.DataBase;
using BackgammonServer.Network;

namespace BackgammonServer.Managers
{
    internal class LogInSignUpManager
    {
        private ConnectionToUserDataBase _db;
        private EncryptedCommunication _encryptedCommunication;

        public LogInSignUpManager(ConnectionToUserDataBase db, EncryptedCommunication encryptedCommunication)
        {
            _db = db;
            _encryptedCommunication = encryptedCommunication;
            _encryptedCommunication.OnMessageReceive += OnMessageReceive;
        }

        private void OnMessageReceive(string message, string ip)
        {
            string[] splitMessage = message.Split(',');
            switch (splitMessage[0])
            {
                case "ValidSignUp":
                    {
                        if (_db.IsUserNameExists(splitMessage[1]))
                        {
                            _encryptedCommunication.SendMessage("SignUp,false,username", ip);
                            break;
                        }
                        if (_db.IsEmailExist(splitMessage[5]))
                        {
                            _encryptedCommunication.SendMessage("SignUp,false,mail", ip);
                            break;
                        }
                        _encryptedCommunication.SendMessage("ValidSignUp", ip);
                        break;

                    }
                case "SignUp":
                    {
                        _db.InsertNewUser(splitMessage[1], splitMessage[2], splitMessage[3], splitMessage[4], splitMessage[5],
                                                                splitMessage[6], splitMessage[7]);
                        _encryptedCommunication.SendMessage("SignUp,true", ip);
                        break;
                    }
                case "Login":
                    {
                        bool isLogin = _db.IsExists(splitMessage[1], splitMessage[2]);

                        if (isLogin)
                        {
                            _encryptedCommunication.SendMessage("Login,true", ip);
                        }
                        else
                        {
                            _encryptedCommunication.SendMessage("Login,false", ip);
                        }
                        break;
                    }
            }
        }
    }
}

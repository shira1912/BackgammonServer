using System.Data.SqlClient;

namespace BackgammonServer.DataBase
{
    public class ConnectionToUserDataBase
    {
        private const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\Shira Noiman\Downloads\BackgammonProject\Backgammon\BackgammonServer\BackgammonServer\DataBase\DataBase.mdf"";Integrated Security=True";
        private SqlConnection connection;
        private SqlCommand command;

        public ConnectionToUserDataBase()
        {
            connection = new SqlConnection(connectionString);
            command = new SqlCommand();
        }

        public void InsertNewUser(string UserName, string Password, string FirstName, string LastName, string Email, string City, string Gender)
        {
            command.CommandText = "INSERT INTO Users (username, password, firstName, lastName, email, city, gender) " +
                                  "VALUES (@username, @password, @firstName, @lastName, @email, @city, @gender)";

            command.Parameters.AddWithValue("@username", UserName);
            command.Parameters.AddWithValue("@password", Password);
            command.Parameters.AddWithValue("@firstName", FirstName);
            command.Parameters.AddWithValue("@lastName", LastName);
            command.Parameters.AddWithValue("@email", Email);
            command.Parameters.AddWithValue("@city", City);
            command.Parameters.AddWithValue("@gender", Gender);

            connection.Open();
            command.Connection = connection;
            int x = command.ExecuteNonQuery();
            connection.Close();
        }

        public bool IsExists(string UserName, string Password)
        {
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE username='" + UserName + "'AND password= '" + Password + "'";
            connection.Open();
            command.Connection = connection;
            int x = (int)command.ExecuteScalar();
            connection.Close();
            if (x > 0)
            {
                return true;
            }
            else { return false; }
        }

        public bool IsUserNameExists(string userName)
        {
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE username='" + userName + "'";
            connection.Open();
            command.Connection = connection;
            int x = (int)command.ExecuteScalar();
            connection.Close();
            if (x > 0)
            {
                return true;
            }
            else { return false; }
        }
    }
}
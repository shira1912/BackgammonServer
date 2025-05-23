using System.Data.SqlClient;

namespace BackgammonServer.DataBase
{
    public class ConnectionToUserDataBase
    {
        private const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\תלמיד מדעים אורח\Desktop\BackgammonProject1\BackgammonProject1.0\BackgammonProject\Backgammon\BackgammonServer\BackgammonServer\DataBase\DataBase.mdf"";Integrated Security=True";
        private SqlConnection connection;
        private SqlCommand command;

        public ConnectionToUserDataBase()
        {
            connection = new SqlConnection(connectionString);
            command = new SqlCommand();
        }

        public void InsertNewUser(string UserName, string Password, string FirstName, string LastName, string Email, string City, string Gender)
        {
            command.CommandText = "INSERT INTO Users (username, password, firstName, lastName, email, city, gender, salt) " +
                                  "VALUES (@username, @password, @firstName, @lastName, @email, @city, @gender, @salt)";

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(Password, salt);

            command.Parameters.AddWithValue("@username", UserName);
            command.Parameters.AddWithValue("@password", hashedPassword);
            command.Parameters.AddWithValue("@firstName", FirstName);
            command.Parameters.AddWithValue("@lastName", LastName);
            command.Parameters.AddWithValue("@email", Email);
            command.Parameters.AddWithValue("@city", City);
            command.Parameters.AddWithValue("@gender", Gender);
            command.Parameters.AddWithValue("@salt", salt);

            connection.Open();
            command.Connection = connection;
            int x = command.ExecuteNonQuery();
            command.Parameters.Clear();
            connection.Close();
        }

        //public bool IsExists(string UserName, string Password)
        //{
        //    command.CommandText = "SELECT COUNT(*) FROM Users WHERE username='" + UserName + "'AND password= '" + Password + "'";
        //    connection.Open();
        //    command.Connection = connection;
        //    int x = (int)command.ExecuteScalar();
        //    connection.Close();
        //    if (x > 0)
        //    {
        //        return true;
        //    }
        //    else { return false; }
        //}
        public bool IsExists(string username, string password)
        {
            command.CommandText = "SELECT password, salt FROM Users WHERE username = @username";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@username", username);

            connection.Open();
            command.Connection = connection;

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    string storedHashedPassword = reader["password"].ToString();
                    string storedSalt = reader["salt"].ToString();

                    string hashedInputPassword = HashPassword(password, storedSalt);

                    connection.Close();
                    return hashedInputPassword == storedHashedPassword;
                }
            }
            return false;
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

        public void resetPasswordByEmail(string email, string newPassword)
        {
            command.CommandText = "UPDATE Users SET password = '" + newPassword + "' WHERE email = '" + email + "'";
            connection.Open();
            command.Connection = connection;
            int x = command.ExecuteNonQuery(); // executes the statement. returns the number of rows affected
            connection.Close();

        }

        public bool IsEmailExist(string email)
        { // gets a username and checks if it's exists in the table
            command.CommandText = " SELECT COUNT(*) FROM Users WHERE email='" + email + "'";
            connection.Open();
            command.Connection = connection;
            int x = (int)command.ExecuteScalar(); // executes the statement. returns the result
            connection.Close();
            if (x > 0)
            {
                return true;
            }
            else { return false; }
        }
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var saltedPassword = password + salt;
                byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
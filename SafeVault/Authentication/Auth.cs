using MySql.Data.MySqlClient;
using static BCrypt.Net.BCrypt;

namespace SafeVault.Authentication
{
    public static class Auth
    {
        // Method to hash a password
        public static string HashPassword(string password)
        {
            // Use BCrypt.Net library to hash the password
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Method to verify a password against a hash
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Use BCrypt.Net library to verify the password
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        // Method to authenticate user
        public static bool AuthenticateUser(string username, string password)
        {
            // Retrieve the hashed password from the database for the given username
            string hashedPasswordFromDb = GetHashedPasswordFromDb(username);

            // Verify the password
            return !string.IsNullOrEmpty(hashedPasswordFromDb) &&
                   VerifyPassword(password, hashedPasswordFromDb);
        }

        // Secure method to retrieve hashed password from the database
        private static string GetHashedPasswordFromDb(string username)
        {
            string query = "SELECT HashedPassword FROM Users WHERE Username = @Username";
            string connectionString = Environment.GetEnvironmentVariable("SAFEVAULT_DB_CONNECTION") 
                                       ?? throw new InvalidOperationException("Database connection string is not set.");

            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                // Add parameterized query to prevent SQL injection
                command.Parameters.Add("@Username", MySqlDbType.VarChar).Value = username;

                connection.Open();
                var result = command.ExecuteScalar();
                // Ensure a non-null return value
                return result?.ToString() ?? string.Empty;
            }
        }
    }
}

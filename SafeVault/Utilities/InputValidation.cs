using MySql.Data.MySqlClient;

namespace SafeVault.Utilities
{
    public static class InputValidation
    {
        public static string SanitizeInput(string input)
        {
            // Basic example for XSS prevention
            // Note: Consider using a library or framework for better security
            input = input.Replace("<", "&lt;").Replace(">", "&gt;");
            input = input.Trim();

            return input;
        }
        
        // Example method for executing parameterized query
        public static void ExecuteParameterizedQuery(string userInput)
        {
            string query = "SELECT * FROM Users WHERE Username = @Username";
            string connectionString = "Server=localhost;Database=SafeVaultDB;User ID=root;Password=yourpassword;"; // Ensure this is a valid connection string

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", userInput);
                
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Process the data
                    }
                }
            }
        }
    }
}

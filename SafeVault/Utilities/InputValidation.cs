using MySql.Data.MySqlClient;
using System.Web;

namespace SafeVault.Utilities
{
    public static class InputValidation
    {
        public static string SanitizeInput(string input)
        {
            return HttpUtility.HtmlEncode(input.Trim());
        }

        public static void ExecuteParameterizedQuery(string userInput)
        {
            string query = "SELECT * FROM Users WHERE Username = @Username";
            string connectionString = Environment.GetEnvironmentVariable("SAFEVAULT_DB_CONNECTION") 
                                       ?? throw new InvalidOperationException("Database connection string is not set.");

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("@Username", MySqlDbType.VarChar).Value = userInput;

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

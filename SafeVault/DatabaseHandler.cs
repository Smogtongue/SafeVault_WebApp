using System.Data.SqlClient;
using SafeVault.Utilities; // Ensure this matches the namespace of InputValidation
using BCrypt.Net; // Add this line

public class DatabaseHandler
{
    private string connectionString = "Server=localhost;Database=SafeVaultDB;User ID=root;Password=yourpassword;";

    public void InsertUser(string username, string email, string password)
    {
        string sanitizedUsername = InputValidation.SanitizeInput(username);
        string sanitizedEmail = InputValidation.SanitizeInput(email);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password); // Hash the password

        string query = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", sanitizedUsername);
            command.Parameters.AddWithValue("@Email", sanitizedEmail);
            command.Parameters.AddWithValue("@Password", hashedPassword); // Add the hashed password

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    public User? GetUser(string username) // Make the return type nullable
    {
        string sanitizedUsername = InputValidation.SanitizeInput(username);

        string query = "SELECT * FROM Users WHERE Username = @Username";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", sanitizedUsername);

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Make sure you're using the correct type conversion methods
                    User user = new User()
                    {
                        UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Email = reader.GetString(reader.GetOrdinal("Email"))
                    };
                    return user;
                }
            }
        }
        return null;
    }
}

public class User
{
    public int UserID { get; set; }
    public string Username { get; set; } = string.Empty; // Initialize with a non-null value
    public string Email { get; set; } = string.Empty; // Initialize with a non-null value
}

using System;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using BCrypt.Net;

namespace SafeVault
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to SafeVault!");

            // Example usage of InputSanitizer
            string userInput = "<script>alert('Hacked!');</script>";
            string sanitizedInput = InputSanitizer.SanitizeInput(userInput);
            Console.WriteLine($"Sanitized Input: {sanitizedInput}");

            string username = "user<script>";
            string sanitizedUsername = InputSanitizer.SanitizeUsername(username);
            Console.WriteLine($"Sanitized Username: {sanitizedUsername}");

            string email = "user@example.com<script>";
            string sanitizedEmail = InputSanitizer.SanitizeEmail(email);
            Console.WriteLine($"Sanitized Email: {sanitizedEmail}");

            // Example usage of DatabaseHelper
            string password = "password123";
            string hashedPassword = DatabaseHelper.HashPassword(password);
            DatabaseHelper.InsertUser(sanitizedUsername, sanitizedEmail, hashedPassword);

            // Assign role to user
            DatabaseHelper.AssignRoleToUser(sanitizedUsername, "user");

            // Example usage of DatabaseHelper for login
            bool loginSuccess = DatabaseHelper.ValidateLogin(sanitizedUsername, password);
            Console.WriteLine($"Login Success: {loginSuccess}");

            // Example usage of DatabaseHelper for retrieving user information
            var userInfo = DatabaseHelper.GetUserInfo(sanitizedUsername);
            Console.WriteLine($"User Info: {userInfo}");

            // Example usage of DatabaseHelper for checking user role
            bool isAdmin = DatabaseHelper.CheckUserRole(sanitizedUsername, "admin");
            Console.WriteLine($"Is Admin: {isAdmin}");

            // Example usage of restricting access to admin dashboard
            if (DatabaseHelper.CheckUserRole(sanitizedUsername, "admin"))
            {
                Console.WriteLine("Access granted to Admin Dashboard.");
            }
            else
            {
                Console.WriteLine("Access denied to Admin Dashboard.");
            }
        }
    }

    public static class InputSanitizer
    {
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Remove potentially harmful characters
            string sanitized = Regex.Replace(input, @"[<>""'/]", string.Empty);

            // Additional sanitization logic can be added here

            return sanitized;
        }

        public static string SanitizeUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return username;
            }

            // Allow only alphanumeric characters and underscores
            string sanitized = Regex.Replace(username, @"[^\w]", string.Empty);

            return sanitized;
        }

        public static string SanitizeEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return email;
            }

            // Remove potentially harmful characters but allow @ and .
            string sanitized = Regex.Replace(email, @"[<>""'/]", string.Empty);

            return sanitized;
        }
    }

    public static class DatabaseHelper
    {
        private static string connectionString = "your_connection_string_here";

        public static void InsertUser(string username, string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool ValidateLogin(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Password FROM Users WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    string storedPasswordHash = (string)command.ExecuteScalar();
                    return BCrypt.Net.BCrypt.Verify(password, storedPasswordHash);
                }
            }
        }

        public static string GetUserInfo(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Username, Email FROM Users WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return $"Username: {reader["Username"]}, Email: {reader["Email"]}";
                        }
                        else
                        {
                            return "User not found.";
                        }
                    }
                }
            }
        }

        public static bool CheckUserRole(string username, string role)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM UserRoles WHERE Username = @Username AND Role = @Role";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Role", role);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count == 1;
                }
            }
        }

        public static void AssignRoleToUser(string username, string role)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO UserRoles (Username, Role) VALUES (@Username, @Role)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Role", role);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Additional methods for parameterized queries can be added here
    }
}

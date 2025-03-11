using System.Collections.Generic;

namespace SafeVault.Authorization
{
    public enum Role { Admin, User }

    public class User
    {
        public string Username { get; set; }
        public Role UserRole { get; set; }

        public User(string username, Role userRole)
        {
            Username = username;
            UserRole = userRole;
        }
    }

    public static class RBAC
    {
        private static Dictionary<string, User> users = new Dictionary<string, User>
        {
            { "admin", new User("admin", Role.Admin) },
            { "user", new User("user", Role.User) }
        };

        public static Role GetUserRole(string username)
        {
            if (users.ContainsKey(username))
            {
                return users[username].UserRole;
            }
            return Role.User; // Default to user role
        }

        public static bool HasAccess(string username, Role requiredRole)
        {
            Role userRole = GetUserRole(username);
            return userRole == requiredRole;
        }
    }
}

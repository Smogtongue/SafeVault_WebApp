// Tests/TestInputValidation.cs
using NUnit.Framework;
using System.Data.SqlClient;

[TestFixture]
public class TestInputValidation
{
    [Test]
    public void TestForSQLInjection()
    {
        string maliciousUsername = "user'; DROP TABLE Users; --";
        string email = "user@example.com";

        try
        {
            DatabaseHelper.InsertUser(maliciousUsername, email, "password123");
            Assert.Fail("SQL injection attempt should have failed.");
        }
        catch (SqlException)
        {
            // Expected exception due to SQL injection attempt
            Assert.IsTrue(true);
        }
    }

    [Test]
    public void TestForXSS()
    {
        string maliciousInput = "<script>alert('Hacked!');</script>";
        string sanitizedInput = InputSanitizer.SanitizeInput(maliciousInput);

        Assert.IsFalse(sanitizedInput.Contains("<script>"), "XSS injection attempt should have been sanitized.");
        Assert.IsFalse(sanitizedInput.Contains("</script>"), "XSS injection attempt should have been sanitized.");
    }

    [Test]
    public void TestSanitizeUsername()
    {
        string maliciousUsername = "user<script>alert('Hacked!');</script>";
        string sanitizedUsername = InputSanitizer.SanitizeUsername(maliciousUsername);

        Assert.IsFalse(sanitizedUsername.Contains("<script>"), "XSS injection attempt should have been sanitized.");
        Assert.IsFalse(sanitizedUsername.Contains("</script>"), "XSS injection attempt should have been sanitized.");
    }

    [Test]
    public void TestSanitizeEmail()
    {
        string maliciousEmail = "user@example.com<script>alert('Hacked!');</script>";
        string sanitizedEmail = InputSanitizer.SanitizeEmail(maliciousEmail);

        Assert.IsFalse(sanitizedEmail.Contains("<script>"), "XSS injection attempt should have been sanitized.");
        Assert.IsFalse(sanitizedEmail.Contains("</script>"), "XSS injection attempt should have been sanitized.");
    }

    [Test]
    public void TestGetUserInfoWithSqlInjection()
    {
        string maliciousUsername = "user' OR '1'='1";
        var userInfo = DatabaseHelper.GetUserInfo(maliciousUsername);

        Assert.AreEqual("User not found.", userInfo, "SQL injection attempt should have failed.");
    }

    [Test]
    public void TestValidateLogin()
    {
        string username = "testuser";
        string password = "password123";

        bool loginSuccess = DatabaseHelper.ValidateLogin(username, password);
        Assert.IsTrue(loginSuccess, "Valid login attempt should have succeeded.");
    }

    [Test]
    public void TestCheckUserRole()
    {
        string username = "adminuser";
        string role = "admin";

        bool isAdmin = DatabaseHelper.CheckUserRole(username, role);
        Assert.IsTrue(isAdmin, "User should have admin role.");
    }

    [Test]
    public void TestAssignRoleToUser()
    {
        string username = "testuser";
        string role = "user";

        DatabaseHelper.AssignRoleToUser(username, role);
        bool hasRole = DatabaseHelper.CheckUserRole(username, role);
        Assert.IsTrue(hasRole, "User should have been assigned the role.");
    }

    [Test]
    public void TestInvalidLogin()
    {
        string username = "nonexistentuser";
        string password = "wrongpassword";

        bool loginSuccess = DatabaseHelper.ValidateLogin(username, password);
        Assert.IsFalse(loginSuccess, "Invalid login attempt should have failed.");
    }

    [Test]
    public void TestUnauthorizedAccess()
    {
        string username = "regularuser";
        string role = "admin";

        bool isAdmin = DatabaseHelper.CheckUserRole(username, role);
        Assert.IsFalse(isAdmin, "Regular user should not have admin access.");
    }

    [Test]
    public void TestAdminAccess()
    {
        string username = "adminuser";
        string role = "admin";

        bool isAdmin = DatabaseHelper.CheckUserRole(username, role);
        Assert.IsTrue(isAdmin, "Admin user should have admin access.");
    }

    [Test]
    public void TestUserAccess()
    {
        string username = "regularuser";
        string role = "user";

        bool isUser = DatabaseHelper.CheckUserRole(username, role);
        Assert.IsTrue(isUser, "Regular user should have user access.");
    }
}
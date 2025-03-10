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
            DatabaseHelper.InsertUser(maliciousUsername, email);
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
}
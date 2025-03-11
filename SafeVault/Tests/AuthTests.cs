using NUnit.Framework;
using SafeVault.Authentication;

[TestFixture]
public class AuthTests
{
    [Test]
    public void TestHashAndVerifyPassword()
    {
        string password = "password123";
        string hash = Auth.HashPassword(password);
        Assert.That(Auth.VerifyPassword(password, hash), Is.True);
    }

    [Test]
    public void TestAuthenticateUser()
    {
        string username = "admin";
        string password = "password123";
        Assert.That(Auth.AuthenticateUser(username, password), Is.True);
    }
}

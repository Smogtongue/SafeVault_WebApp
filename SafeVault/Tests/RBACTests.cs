using NUnit.Framework;
using SafeVault.Authorization;

[TestFixture]
public class RBACTests
{
    [Test]
    public void TestAdminAccess()
    {
        string username = "admin";
        Assert.That(RBAC.HasAccess(username, Role.Admin), Is.True);
    }

    [Test]
    public void TestUserAccess()
    {
        string username = "user";
        Assert.That(RBAC.HasAccess(username, Role.Admin), Is.False);
    }

    [Test]
    public void TestInvalidAccess()
    {
        string username = "unknown";
        Assert.That(RBAC.HasAccess(username, Role.Admin), Is.False);
    }
}

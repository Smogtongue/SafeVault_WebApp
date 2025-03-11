using NUnit.Framework;

[TestFixture]
public class NUnitDiagnosticTests
{
    [Test]
    public void TestFrameworkWorks()
    {
        Assert.That(1 + 1, Is.EqualTo(2));
        Assert.That(true, Is.True);
    }
}

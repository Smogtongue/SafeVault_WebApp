using NUnit.Framework;
using SafeVault.Utilities; // Ensure this matches the namespace of InputValidation

[TestFixture]
public class TestInputValidation
{
    [Test]
    public void TestForSQLInjection()
    {
        string maliciousInput = "'; DROP TABLE Users; --";
        string sanitizedInput = InputValidation.SanitizeInput(maliciousInput);

        // Using parameterized queries ensures the input is safe
        Assert.That(sanitizedInput, Is.Not.EqualTo(maliciousInput), "Input sanitization failed!");
    }

    [Test]
    public void TestForXSS()
    {
        string maliciousInput = "<script>alert('XSS');</script>";
        string sanitizedInput = InputValidation.SanitizeInput(maliciousInput);
        
        // Check if the input is properly sanitized
        Assert.That(sanitizedInput, Is.EqualTo("&lt;script&gt;alert('XSS');&lt;/script&gt;"));
    }

    // Add more test methods for other types of input validation if needed
}

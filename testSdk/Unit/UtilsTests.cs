using RigelSdk.src.Utils;

namespace Tests.Unit
{
    /// <summary>
    /// Unit tests for utility classes
    /// </summary>
    public class UtilsTests
    {
        [Fact]
        public void SignatureUtils_Sign_ReturnsCorrectSignature()
        {
            // Arrange
            const string key = "secretkey";
            const string salt = "secretsalt";
            const string input = "test-input";

            // Act
            var signature = SignatureUtils.Sign(key, salt, input);

            // Assert
            signature.Should().NotBeNullOrEmpty();
            signature.Should().NotContain("+");
            signature.Should().NotContain("/");
            signature.Should().NotContain("=");
        }

        [Fact]
        public void SignatureUtils_SignQueryString_WithoutExpiry_ReturnsCorrectSignature()
        {
            // Arrange
            const string key = "secretkey";
            const string salt = "secretsalt";
            const string requestPath = "proxy";
            const string queryString = "img=https://example.com/image.jpg";

            // Act
            var result = SignatureUtils.SignQueryString(key, salt, requestPath, queryString, -1);

            // Assert
            result.Should().Contain("img=https://example.com/image.jpg");
            result.Should().Contain("X-Signature=");
            result.Should().NotContain("X-ExpiresAt");
        }

        [Fact]
        public void SignatureUtils_SignQueryString_WithExpiry_IncludesExpiryTime()
        {
            // Arrange
            const string key = "secretkey";
            const string salt = "secretsalt";
            const string requestPath = "proxy";
            const string queryString = "img=https://example.com/image.jpg";
            const long expiry = 86400000;

            // Act
            var result = SignatureUtils.SignQueryString(key, salt, requestPath, queryString, expiry);

            // Assert
            result.Should().Contain("X-ExpiresAt=86400000");
            result.Should().Contain("X-Signature=");
        }

        [Fact]
        public void QueryStringUtils_SerializeToQueryString_ReturnsCorrectFormat()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "width", "100" },
                { "height", "200" },
                { "type", "WEBP" }
            };

            // Act
            var result = QueryStringUtils.SerializeToQueryString(parameters);

            // Assert
            result.Should().Contain("width=100");
            result.Should().Contain("height=200");
            result.Should().Contain("type=WEBP");
            result.Split('&').Should().HaveCount(3);
        }
    }
}

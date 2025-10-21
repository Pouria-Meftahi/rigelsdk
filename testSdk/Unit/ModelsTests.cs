using RigelSdk.src.Constants;
using RigelSdk.src.Models;

namespace Tests.Unit
{
    /// <summary>
    /// Unit tests for model classes
    /// </summary>
    public class ModelsTests
    {
        [Fact]
        public void Options_ToQueryString_WithAllProperties_ReturnsCorrectString()
        {
            // Arrange
            var options = new Options
            {
                Width = 100,
                Height = 200,
                Type = ImageType.WEBP,
                Quality = 80,
                Crop = true
            };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            queryString.Should().Contain("width=100");
            queryString.Should().Contain("height=200");
            queryString.Should().Contain("type=2");
            queryString.Should().Contain("quality=80");
            queryString.Should().Contain("crop=");
        }

        [Fact]
        public void Options_ToQueryString_WithNullProperties_OmitsNullValues()
        {
            // Arrange
            var options = new Options
            {
                Width = 100,
                Height = null,
                Type = null
            };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            queryString.Should().Contain("width=100");
            queryString.Should().NotContain("height=");
            queryString.Should().NotContain("type=");
        }

        [Fact]
        public void ProxyParams_Constructor_WithParameters_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var proxyParams = new ProxyParams("https://example.com/image.jpg", new Options { Width = 100 });

            // Assert
            proxyParams.Img.Should().Be("https://example.com/image.jpg");
            proxyParams.Options.Width.Should().Be(100);
        }

        [Fact]
        public void CacheImageResponse_Deserialization_WorksCorrectly()
        {
            // Arrange
            var json = @"{
                ""img"": ""https://example.com/image.jpg"",
                ""signature"": ""abc123"",
                ""short_url"": ""http://localhost/img/abc123""
            }";

            // Act
            var response = JsonSerializer.Deserialize<CacheImageResponse>(json);

            // Assert
            response.Should().NotBeNull();
            response!.Img.Should().Be("https://example.com/image.jpg");
            response.Signature.Should().Be("abc123");
            response.ShortUrl.Should().Be("http://localhost/img/abc123");
        }
    }
}

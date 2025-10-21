using RigelSdk.src;
using RigelSdk.src.Constants;
using RigelSdk.src.Models;
using Tests.Helpers;

namespace Tests.Integration
{
    /// <summary>
    /// Integration tests for RigelSdk (requires running Rigel server)
    /// </summary>
    [Trait("Category", "Integration")]
    public class SdkIntegrationTests
    {
        private readonly Sdk _sdk;

        public SdkIntegrationTests()
        {
            _sdk = new Sdk(TestConstants.BaseUrl, TestConstants.Key, TestConstants.Salt);
        }

        [Fact(Skip = "Requires running Rigel server")]
        public async Task ProxyImageAsync_WithRealServer_ReturnsValidUrl()
        {
            // Arrange
            var options = new Options
            {
                Width = 100,
                Height = 100,
                Type = ImageType.WEBP
            };

            // Act
            var result = await _sdk.ProxyImageAsync(TestConstants.TestImageUrl, options, -1);

            // Assert
            result.Should().StartWith(TestConstants.BaseUrl);
            result.Should().Contain("X-Signature=");
        }

        [Fact(Skip = "Requires running Rigel server")]
        public async Task CacheImageAsync_WithRealServer_ReturnsShortUrl()
        {
            // Arrange
            var options = new Options
            {
                Width = 300,
                Height = 300,
                Type = ImageType.WEBP
            };

            // Act
            var result = await _sdk.CacheImageAsync(TestConstants.TestImageUrl, options, -1);

            // Assert
            result.Should().BeOfType<string>();
            ((string)result).Should().StartWith(TestConstants.BaseUrl);
        }

        [Fact(Skip = "Requires running Rigel server")]
        public async Task BatchedCacheImageAsync_WithRealServer_ReturnsMultipleUrls()
        {
            // Arrange
            var proxyParams = new List<ProxyParams>
            {
                new ProxyParams(TestConstants.TestImageUrl, new Options { Width = 100, Height = 100, Type = ImageType.WEBP }),
                new ProxyParams(TestConstants.TestImageUrl2, new Options { Width = 100, Height = 100, Type = ImageType.WEBP })
            };

            // Act
            var result = await _sdk.BatchedCacheImageAsync(proxyParams, -1);

            // Assert
            result.Should().HaveCount(2);
            result.All(r => !string.IsNullOrEmpty(r.ShortUrl)).Should().BeTrue();
        }
    }
}

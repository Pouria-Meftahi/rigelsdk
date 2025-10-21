using RigelSdk.src;
using RigelSdk.src.Constants;
using RigelSdk.src.Models;
using System.Net;
using Tests.Helpers;

namespace Tests.Unit
{
    /// <summary>
    /// Unit tests for RigelSdk class
    /// </summary>
    public class SdkTests
    {
        private readonly Sdk _sdk;

        public SdkTests()
        {
            _sdk = new Sdk(TestConstants.BaseUrl, TestConstants.Key, TestConstants.Salt);
        }

        [Fact]
        public async Task ProxyImageAsync_WithoutOptionsAndExpiry_ReturnsCorrectUrl()
        {
            // Act
            var result = await _sdk.ProxyImageAsync(TestConstants.TestImageUrl, null, -1);

            // Assert
            result.Should().Be(
                "http://localhost:8080/rigel/proxy?img=https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg&X-Signature=vX59TgdwdNqZD_jXGOky_zVgttc"
            );
        }

        [Fact]
        public async Task ProxyImageAsync_WithOptionsWithoutExpiry_ReturnsCorrectUrl()
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
            result.Should().Be(
                "http://localhost:8080/rigel/proxy?height=100&img=https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg&type=2&width=100&X-Signature=zkEmP1FDNoopC8GoM-caGzx1_1s"
            );
        }

        [Fact]
        public async Task ProxyImageAsync_WithOptionsAndExpiry_ReturnsCorrectUrl()
        {
            // Arrange
            var options = new Options
            {
                Width = 100,
                Height = 100,
                Type = ImageType.WEBP
            };
            var expiry = 1000 * 60 * 60 * 24; // 1 day in milliseconds

            // Act
            var result = await _sdk.ProxyImageAsync(TestConstants.TestImageUrl, options, expiry);

            // Assert
            result.Should().Contain("X-ExpiresAt=86400000");
            result.Should().Contain("height=100");
            result.Should().Contain("width=100");
            result.Should().Contain("type=2");
            result.Should().Contain("X-Signature=");
        }

        [Fact]
        public async Task ProxyImageAsync_WithComplexOptions_ReturnsCorrectUrl()
        {
            // Arrange
            var options = new Options
            {
                Width = 200,
                Height = 200,
                Quality = 80,
                Crop = true,
                Type = ImageType.PNG,
                Gravity = Gravity.Centre
            };

            // Act
            var result = await _sdk.ProxyImageAsync(TestConstants.TestImageUrl, options, -1);

            // Assert
            result.Should().Contain("width=200");
            result.Should().Contain("height=200");
            result.Should().Contain("quality=80");
            result.Should().Contain("crop=");
            result.Should().Contain("X-Signature=");
        }

        [Fact]
        public async Task CacheImageAsync_Success_ReturnsShortUrl()
        {
            // Arrange
            var mockResponse = new CacheImageResponse
            {
                Img = TestConstants.TestImageUrl,
                Signature = "fde5eda7214568293ad70621aec2ad1efee5c7fd",
                ShortUrl = ""
            };
            var mockHttpClient = MockHttpMessageHandler.CreateMockHttpClient(
                HttpStatusCode.OK,
                JsonSerializer.Serialize(mockResponse)
            );
            var sdk = new Sdk(TestConstants.BaseUrl, TestConstants.Key, TestConstants.Salt, mockHttpClient);
            var options = new Options { Width = 300, Height = 300, Type = ImageType.WEBP };

            // Act
            var result = await sdk.CacheImageAsync(TestConstants.TestImageUrl, options, -1);

            // Assert
            result.Should().BeOfType<string>();
            ((string)result).Should().Contain("/img/fde5eda7214568293ad70621aec2ad1efee5c7fd");
            ((string)result).Should().Contain("X-Signature=");
        }

        [Fact]
        public async Task CacheImageAsync_ServerError_ReturnsStatusCode()
        {
            // Arrange
            var mockHttpClient = MockHttpMessageHandler.CreateMockHttpClient(
                HttpStatusCode.ServiceUnavailable,
                ""
            );
            var sdk = new Sdk(TestConstants.BaseUrl, TestConstants.Key, TestConstants.Salt, mockHttpClient);
            var options = new Options { Width = 300, Height = 300, Type = ImageType.WEBP };

            // Act
            var result = await sdk.CacheImageAsync(TestConstants.TestImageUrl, options, -1);

            // Assert
            result.Should().BeOfType<int>();
            ((int)result).Should().Be(503);
        }

        [Fact]
        public async Task BatchedCacheImageAsync_WithMultipleImages_ReturnsCorrectResponses()
        {
            // Arrange
            var expectedResponses = new List<CacheImageResponse>
            {
                new CacheImageResponse
                {
                    Img = TestConstants.TestImageUrl,
                    Signature = "124799fa1f5d2069e1b56793e01f8fe260b87791",
                    ShortUrl = ""
                },
                new CacheImageResponse
                {
                    Img = TestConstants.TestImageUrl2,
                    Signature = "7fba571dee9007af7964e23239e2a1201419c0b8",
                    ShortUrl = ""
                }
            };

            var mockHttpClient = MockHttpMessageHandler.CreateMockHttpClient(
                HttpStatusCode.OK,
                JsonSerializer.Serialize(expectedResponses)
            );
            var sdk = new Sdk(TestConstants.BaseUrl, TestConstants.Key, TestConstants.Salt, mockHttpClient);

            var proxyParams = new List<ProxyParams>
            {
                new ProxyParams(TestConstants.TestImageUrl, new Options { Width = 100, Height = 100, Type = ImageType.WEBP }),
                new ProxyParams(TestConstants.TestImageUrl2, new Options { Width = 100, Height = 100, Type = ImageType.WEBP })
            };

            // Act
            var result = await sdk.BatchedCacheImageAsync(proxyParams, -1);

            // Assert
            result.Should().HaveCount(2);
            result[0].Img.Should().Be(TestConstants.TestImageUrl);
            result[0].ShortUrl.Should().Contain("/img/124799fa1f5d2069e1b56793e01f8fe260b87791");
            result[1].Img.Should().Be(TestConstants.TestImageUrl2);
            result[1].ShortUrl.Should().Contain("/img/7fba571dee9007af7964e23239e2a1201419c0b8");
        }

        [Fact]
        public async Task BatchedCacheImageAsync_ServerError_ThrowsException()
        {
            // Arrange
            var mockHttpClient = MockHttpMessageHandler.CreateMockHttpClient(
                HttpStatusCode.InternalServerError,
                ""
            );
            var sdk = new Sdk(TestConstants.BaseUrl, TestConstants.Key, TestConstants.Salt, mockHttpClient);
            var proxyParams = new List<ProxyParams>
            {
                new ProxyParams(TestConstants.TestImageUrl, new Options { Width = 100, Height = 100 })
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await sdk.BatchedCacheImageAsync(proxyParams, -1)
            );
        }

        [Fact]
        public async Task TryShortUrlAsync_CacheSuccess_ReturnsShortUrl()
        {
            // Arrange
            var mockResponse = new CacheImageResponse
            {
                Img = TestConstants.TestImageUrl,
                Signature = "fde5eda7214568293ad70621aec2ad1efee5c7fd",
                ShortUrl = ""
            };
            var mockHttpClient = MockHttpMessageHandler.CreateMockHttpClient(
                HttpStatusCode.OK,
                JsonSerializer.Serialize(mockResponse)
            );
            var sdk = new Sdk(TestConstants.BaseUrl, TestConstants.Key, TestConstants.Salt, mockHttpClient);
            var options = new Options { Width = 300, Height = 300, Type = ImageType.WEBP };

            // Act
            var result = await sdk.TryShortUrlAsync(TestConstants.TestImageUrl, options, -1);

            // Assert
            result.Should().Contain("/img/fde5eda7214568293ad70621aec2ad1efee5c7fd");
            result.Should().Contain("X-Signature=");
        }

        [Fact]
        public async Task TryShortUrlAsync_CacheFails_ReturnsProxyUrl()
        {
            // Arrange
            var mockHttpClient = MockHttpMessageHandler.CreateMockHttpClient(
                HttpStatusCode.ServiceUnavailable,
                ""
            );
            var sdk = new Sdk(TestConstants.BaseUrl, TestConstants.Key, TestConstants.Salt, mockHttpClient);
            var options = new Options { Width = 300, Height = 300, Type = ImageType.WEBP };

            // Act
            var result = await sdk.TryShortUrlAsync(TestConstants.TestImageUrl, options, -1);

            // Assert
            result.Should().Contain("/proxy?");
            result.Should().Contain("X-Signature=");
        }
    }
}

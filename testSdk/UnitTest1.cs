using rigelsdk.src;
using System.Text.Json;

namespace testSdk;

public class UnitTest1
{
    private readonly string key = "secretkey";
    private readonly string salt = "secretsalt";
    private readonly string baseURL = "http://localhost:8080/rigel";
    private readonly string imageURL = "https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg";

    [Fact]
    public void TestProxyImageWithoutOptionsAndExpiry()
    {
        var rigelInstance = new Sdk(baseURL, key, salt);
        var result = rigelInstance.ProxyImage("https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg", null, -1);
        Assert.Equal("http://localhost:8080/rigel/proxy?img=https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg&X-Signature=vX59TgdwdNqZD_jXGOky_zVgttc", result);
    }

    [Fact]
    public void TestProxyImageWithOptionsWithoutExpiry()
    {
        var rigelInstance = new Sdk(baseURL, key, salt);

        var actual = rigelInstance.ProxyImage(imageURL, new Options { Width = 100, Height = 100, Type = ImageType.WEBP }, -1);
        //http://localhost:8080/rigel/proxy?img=https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg&height=100&width=100&type=WEBP&X-Signature=6SvZmn_nVHwA2pUSBBcfzJYpLpM
        Assert.Equal($"{baseURL}/proxy?img=https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg&height=100&width=100&type=WEBP&X-Signature=6SvZmn_nVHwA2pUSBBcfzJYpLpM", actual);
    }

    [Fact]
    public void TestProxyImageWithOptionsAndExpiry()
    {
        var rigelInstance = new Sdk(baseURL, key, salt);

        var actual = rigelInstance.ProxyImage(imageURL, new Options { Width = 100, Height = 100, Type = ImageType.WEBP }, 1000 * 60 * 60 * 24);

        Assert.Equal($"{baseURL}/proxy?img={imageURL}&height=100&width=100&type=WEBP&X-ExpiresAt=86400000&X-Signature=s4XLU2IelRqQbHsGf3JXAibEVeQ", actual);
    }

    [Fact]
    public void TestCacheImage()
    {
        var rigelInstance = new Sdk(baseURL, key, salt);
        string imageURL = "https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg";
        //string expected = $"{baseURL}/img/fde5eda7214568293ad70621aec2ad1efee5c7fd?X-Signature=ztW09e3EvM5IE7fJNsg0Z5-lPXg";
        string expected = $"{baseURL}/img/fde5eda7214568293ad70621aec2ad1efee5c7fd?X-Signature=AsDacUxLR_s9eiAPm1EPkArLtv4";
        var result = rigelInstance.CacheImage(imageURL, new Options { Width = 100, Height = 100, Type = ImageType.WEBP }, -1);
        Assert.Equal(expected, result);
    }
    [Fact]
    public void TestBatchedCacheImage()
    {

        var rigleInstance = new Sdk(baseURL, key, salt);

        List<ProxyParams> batchedCachedImageArgs = new()
        {
            new ProxyParams("https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg", new Options { Height = 100, Width = 100, Type = ImageType.WEBP } ),
            new ProxyParams("https://img.freepik.com/premium-photo/baby-cat-british-shorthair_648604-47.jpg", new Options { Height = 100, Width = 100, Type = ImageType.WEBP } ),
        };

        try
        {
            var result = rigleInstance.BatchedCacheImage(batchedCachedImageArgs, -1);
            var expected = new List<object> {
                new  {
                    img = "https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg",
                    signature= "124799fa1f5d2069e1b56793e01f8fe260b87791",
                    short_url="http://localhost:8080/rigel/img/124799fa1f5d2069e1b56793e01f8fe260b87791?X-Signature=oLiW_5SZmf-13KP2eYv1lisJNCI"
                },
                new {
                img = "https://img.freepik.com/premium-photo/baby-cat-british-shorthair_648604-47.jpg",
                signature = "7fba571dee9007af7964e23239e2a1201419c0b8",
                short_url =
                "http://localhost:8080/rigel/img/7fba571dee9007af7964e23239e2a1201419c0b8?X-Signature=8kFZt2kYHTQoUNqPEzleTn7a6QI",
               }
            };
            Assert.Equal(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(result));
        }
        catch
        {
            //TASK:ADD exception

            throw;
        }

    }
    [Fact]
    public void TryShortURL()
    {
        var rigleInstance = new Sdk(baseURL, key, salt);

        var shortURL = rigleInstance.TryShortURL("https://www.pakainfo.com/wp-content/uploads/2021/09/image-url-for-testing.jpg", new Options { Width = 300, Height = 300, Type = ImageType.WEBP }, -1);

        Assert.Equal(shortURL, "http://localhost:8080/rigel/img/fde5eda7214568293ad70621aec2ad1efee5c7fd?X-Signature=ztW09e3EvM5IE7fJNsg0Z5-lPXg");
    }
}
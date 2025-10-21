using RigelSdk.src.Models;

namespace RigelSdk.src
{
    /// <summary>
    /// Interface for Rigel SDK operations
    /// </summary>
    public interface ISdk
    {
        Task<string> ProxyImageAsync(string imageUrl, Options? options, long expiry);
        Task<object> CacheImageAsync(string imageUrl, Options? options, long expiry);
        Task<List<CacheImageResponse>> BatchedCacheImageAsync(List<ProxyParams> proxyParams, long expiry);
        Task<string> TryShortUrlAsync(string imageUrl, Options? options, long expiry);
    }
}

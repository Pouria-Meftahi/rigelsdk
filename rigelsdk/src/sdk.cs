using RigelSdk.src.Models;
using RigelSdk.src.Utils;
using System.Net.Http.Json;

namespace RigelSdk.src
{
    /// <summary>
    /// Main SDK class for interacting with Rigel image proxy service
    /// </summary>
    public class Sdk : ISdk
    {
        private readonly string _baseUrl;
        private readonly string _key;
        private readonly string _salt;
        private readonly HttpClient _httpClient;

        public Sdk(string baseUrl, string key, string salt)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _key = key;
            _salt = salt;
            _httpClient = new HttpClient();
        }

        public Sdk(string baseUrl, string key, string salt, HttpClient httpClient)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _key = key;
            _salt = salt;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Generates a proxy URL for an image with optional transformations
        /// </summary>
        public Task<string> ProxyImageAsync(string imageUrl, Options? options, long expiry)
        {
            var queryString = BuildQueryString(imageUrl, options);
            var signedQueryString = SignatureUtils.SignQueryString(_key, _salt, "proxy", queryString, expiry);
            var pathUrl = $"{_baseUrl}/proxy?{signedQueryString}";

            return Task.FromResult(pathUrl);
        }

        /// <summary>
        /// Caches an image and returns a short URL or status code
        /// </summary>
        public async Task<object> CacheImageAsync(string imageUrl, Options? options, long expiry)
        {
            var queryString = BuildQueryString(imageUrl, options);
            var signedQueryString = SignatureUtils.SignQueryString(_key, _salt, "headsup", queryString, expiry);
            var pathUrl = $"{_baseUrl}/headsup?{signedQueryString}";

            try
            {
                var response = await _httpClient.PostAsync(pathUrl, null);

                if (!response.IsSuccessStatusCode)
                {
                    return (int)response.StatusCode;
                }

                var imageResponse = await response.Content.ReadFromJsonAsync<CacheImageResponse>();

                if (imageResponse == null)
                {
                    return 503;
                }

                var sqs = SignatureUtils.SignQueryString(_key, _salt, $"img/{imageResponse.Signature}", "", expiry);
                var url = $"{_baseUrl}/img/{imageResponse.Signature}?{sqs}";

                return url;
            }
            catch (Exception)
            {
                return 503;
            }
        }

        /// <summary>
        /// Caches multiple images in a single batch request
        /// </summary>
        public async Task<List<CacheImageResponse>> BatchedCacheImageAsync(List<ProxyParams> proxyParams, long expiry)
        {
            var signedQueryString = SignatureUtils.SignQueryString(_key, _salt, "batched-headsup", "", expiry);
            var pathUrl = $"{_baseUrl}/batched-headsup?{signedQueryString}";

            try
            {
                var response = await _httpClient.PostAsJsonAsync(pathUrl, proxyParams);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed when caching image with status code = {response.StatusCode}");
                }

                var result = await response.Content.ReadFromJsonAsync<List<CacheImageResponse>>();

                if (result == null)
                {
                    throw new Exception("Failed to deserialize response");
                }

                foreach (var cacheImageResponse in result)
                {
                    var sqs = SignatureUtils.SignQueryString(_key, _salt, $"img/{cacheImageResponse.Signature}", "", expiry);
                    cacheImageResponse.ShortUrl = $"{_baseUrl}/img/{cacheImageResponse.Signature}?{sqs}";
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Batched cache operation failed", ex);
            }
        }

        /// <summary>
        /// Tries to create a short URL, falls back to proxy URL if caching fails
        /// </summary>
        public async Task<string> TryShortUrlAsync(string imageUrl, Options? options, long expiry)
        {
            var url = await CacheImageAsync(imageUrl, options, expiry);

            if (url is string shortUrl)
            {
                return shortUrl;
            }

            return await ProxyImageAsync(imageUrl, options, expiry);
        }

        private string BuildQueryString(string imageUrl, Options? options)
        {
            var baseQuery = QueryStringUtils.SerializeToQueryString(new Dictionary<string, string>
            {
                { "img", imageUrl }
            });

            if (options != null)
            {
                var optionsQuery = options.ToQueryString();
                if (!string.IsNullOrEmpty(optionsQuery))
                {
                    return $"{baseQuery}&{optionsQuery}";
                }
            }

            return baseQuery;
        }
    }
}

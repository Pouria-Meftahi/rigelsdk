using System.Text.Json.Serialization;

namespace RigelSdk.src.Models
{
    /// <summary>
    /// Response from cache image operations
    /// </summary>
    public class CacheImageResponse
    {
        [JsonPropertyName("img")]
        public string Img { get; set; } = string.Empty;

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = string.Empty;

        [JsonPropertyName("short_url")]
        public string ShortUrl { get; set; } = string.Empty;
    }
}

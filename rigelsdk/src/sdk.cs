using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace rigelsdk.src
{
    public class Sdk
    {
        public string BaseURL { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public Sdk(string baseURL, string key, string salt)
        {
            BaseURL = baseURL;
            Key = key;
            Salt = salt;
        }

        public string ProxyImage(string imageURL, Options? options, long expiry)
        {
            string queryString;
            Dictionary<string, string> myDictionary = new()
            {
                { "img", imageURL }
            };

            if (options is not null && options.QueryString() is not null)
            {
                queryString = Utils.SerializeToQuryString(myDictionary) + '&' + options.QueryString();
            }
            else
            {
                queryString = Utils.SerializeToQuryString(myDictionary);
            }
            string? signedQueryString = Utils.SignQueryString(Key, Salt, "proxy", queryString, expiry);
            string pathURL = $"{BaseURL}/proxy?{signedQueryString}";
            return pathURL;
        }

        public  string CacheImage(string imageURL, Options options, long expiry)
        {
            string queryString = string.Empty;
            Dictionary<string, string> myDictionary = new()
            {
                { "img", imageURL }
            };
            if (options is not null && options.QueryString() != "")
            {
                queryString = Utils.SerializeToQuryString(myDictionary) + "&" + options.QueryString();
            }
            else
            {
                queryString = Utils.SerializeToQuryString(myDictionary);
            }
            string signedQueryString = Utils.SignQueryString(Key, Salt, "headsup", queryString, expiry);
            string pathURL = $"{BaseURL}/headsup?{signedQueryString}";

            try
            {
                var client = new HttpClient();
                // this endpoint return 400 !
                HttpResponseMessage response = client.PostAsync(pathURL, null).Result;

                var imageResponse = JsonSerializer.Deserialize<CacheImageResponse>(response.Content.ReadAsStringAsync().Result);
                if (response.IsSuccessStatusCode)
                {
                    var sqs = Utils.SignQueryString(Key, Salt, $"img{imageResponse.signature}", queryString, expiry);
                    string URL = $"{BaseURL}/img/{imageResponse.signature}?{sqs}";
                    return URL;
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
            catch
            {
                //TODO:Add loger
                return 503.ToString();
            }
        }

        public async Task<IEnumerable<CacheImageResponse>> BatchedCacheImage(List<ProxyParams> proxyParamsList, long expiry)
        {

            var signedQueryString = Utils.SignQueryString(Key, Salt, "batched-headsup", "", expiry);
            string pathURL = $"{BaseURL}/batched-headsup?{signedQueryString}";
            try
            {
                var client = new HttpClient();
                var response = await client.PostAsJsonAsync(pathURL, proxyParamsList);
                if (response.IsSuccessStatusCode)
                {
                    // TODO: Fix this to check for whether data satisfies interface or not
                    IEnumerable<CacheImageResponse> result = (IEnumerable<CacheImageResponse>)response.Content.ReadAsStringAsync();
                    foreach (var item in result)
                    {
                        var sqs = Utils.SignQueryString(Key, Salt, $"img/{item.signature}", "", expiry);
                        item.short_url = $"{BaseURL}/img/{item.signature}?{sqs}";
                    }
                    return result;
                }
                else
                {
                    throw new($"Failed when caching image with status code = {response.StatusCode}");
                }
            }
            catch
            {
                throw;
            }
        }

        public dynamic TryShortURL(string imageURL, Options options, long expiry)
        {
            var pathUrl = CacheImage(imageURL, options, expiry);
            if (pathUrl is string)
            {
                return pathUrl;
            }
            return ProxyImage(imageURL, options, expiry);
        }
    }
}
namespace RigelSdk.src.Models
{
    /// <summary>
    /// Parameters for proxy image requests
    /// </summary>
    public class ProxyParams
    {
        public string Img { get; set; } = string.Empty;
        public Options Options { get; set; } = new Options();

        public ProxyParams() { }

        public ProxyParams(string img, Options options)
        {
            Img = img;
            Options = options;
        }
    }
}

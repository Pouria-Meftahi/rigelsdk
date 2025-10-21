namespace rigelsdk.src
{
    public class CacheImageResponse
    {
        public string img { get; set; }
        public string signature { get; set; }
        public string short_url { get; set; }
    }

    public class ProxyParams
    {
        public string Img { get; set; } = string.Empty;
        public Options Options { get; set; } = new Options();

        public ProxyParams(string img, Options options)
        {
            Img = img;
            Options = options;
        }
    }

    public class Options
    {
        public int? Height { get; set; }
        public int? Width { get; set; }
        public int? AreaHeight { get; set; }
        public int? AreaWidth { get; set; }
        public int? Top { get; set; }
        public int? Left { get; set; }
        public int? Quality { get; set; }
        public int? Compression { get; set; }
        public int? Zoom { get; set; }
        public bool? Crop { get; set; }
        public bool? Enlarge { get; set; }
        public bool? Embed { get; set; }
        public bool? Flip { get; set; }
        public bool? Flop { get; set; }
        public bool? Force { get; set; }
        public bool? NoAutoRotate { get; set; }
        public bool? NoProfile { get; set; }
        public bool? Interlace { get; set; }
        public bool? StripMetadata { get; set; }
        public bool? Trim { get; set; }
        public bool? Lossless { get; set; }
        public string Extend { get; set; }
        public string Rotate { get; set; }
        public string Background { get; set; }
        public Gravity? Gravity { get; set; } = null;
        public string Watermark { get; set; }
        public string WatermarkImage { get; set; }
        public ImageType? Type { get; set; } = null;
        public string Interpolator { get; set; }
        public string Format { get; set; }
        public string Page { get; set; }
        public string Output { get; set; }
        public string QualityMode { get; set; }
        public string CompressionMode { get; set; }
        public string CacheBuster { get; set; }

        public Options(Options? source = null)
        {
            if (source != null)
            {
                foreach (var prop in this.GetType().GetProperties())
                {
                    //prop.SetValue(source, prop.GetValue(this, null));
                    prop.SetValue(this, source, null);
                }
            }
        }

        public string QueryString()
        {
            var queryParams = new List<string>();
            foreach (var item in this.GetType().GetProperties())
            {
                if (item.CanRead && item.GetValue(this)!= null)
                {
                    queryParams.Add(Uri.EscapeDataString(item.Name.ToLower()) + "=" + Uri.EscapeDataString(item.GetValue(this)?.ToString()));
                }
            }
            return string.Join("&", queryParams);
        }
    }
}

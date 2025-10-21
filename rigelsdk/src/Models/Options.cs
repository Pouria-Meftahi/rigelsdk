using RigelSdk.src.Constants;
using System.Web;
namespace RigelSdk.src.Models
{
    /// <summary>
    /// Image processing options for Rigel
    /// </summary>
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
        public string? Extend { get; set; }
        public string? Rotate { get; set; }
        public string? Background { get; set; }
        public Gravity? Gravity { get; set; }
        public string? Watermark { get; set; }
        public string? WatermarkImage { get; set; }
        public ImageType? Type { get; set; }
        public string? Interpolator { get; set; }
        public string? Interpretation { get; set; }
        public string? GaussianBlur { get; set; }
        public string? Sharpen { get; set; }
        public double? Threshold { get; set; }
        public double? Gamma { get; set; }
        public double? Brightness { get; set; }
        public double? Contrast { get; set; }
        public string? OutputICC { get; set; }
        public string? InputICC { get; set; }
        public bool? Palette { get; set; }

        /// <summary>
        /// Converts options to query string format
        /// </summary>
        public string ToQueryString()
        {
            var properties = GetType().GetProperties()
                .Where(p => p.GetValue(this) != null)
                .Select(p => new KeyValuePair<string, string>(
                    p.Name.ToLowerInvariant(),
                    HttpUtility.UrlEncode(p.GetValue(this)?.ToString())
                ));

            return string.Join("&", properties.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }
    }
}

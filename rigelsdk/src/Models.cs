using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rigelsdk.src
{
    public interface CacheImageResponse
    {
        public string img { get; set; }
        public string signature { get; set; }
        public string short_url { get; set; }
    }
}

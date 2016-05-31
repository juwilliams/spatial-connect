using System.Collections.Generic;

namespace SpatialConnect.Entity
{
    public sealed class Fields
    {
        public string path { get; set; }
        public string type { get; set; }
        public List<Mapping> mappings { get; set; }
    }
}

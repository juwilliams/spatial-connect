namespace SpatialConnect.Entity
{
    public sealed class Mapping
    {
        public string tags { get; set; }
        public string field_to { get; set; }
        public string field_from { get; set; }
        public string length { get; set; }
        public string type { get; set; }

        public bool HasTag(string type)
        {
            return false;
        }
    }
}

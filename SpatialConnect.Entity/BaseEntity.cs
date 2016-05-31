using Newtonsoft.Json;
using System.IO;

namespace SpatialConnect.Entity
{
    public abstract class BaseEntity
    {
        [JsonProperty("py/object")]
        public string py_object { get; set; }

        public void Write(string path)
        {
            string json = JsonConvert.SerializeObject(this);

            File.WriteAllText(path, json);
        }
    }
}

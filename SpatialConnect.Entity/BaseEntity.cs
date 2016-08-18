using Newtonsoft.Json;
using System.IO;

namespace SpatialConnect.Entity
{
    public abstract class BaseEntity
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty("py/object")]
        public string py_object { get; set; }

        public void Write(string path)
        {
            string json = JsonConvert.SerializeObject(this);

            File.WriteAllText(path, json);
        }
    }
}

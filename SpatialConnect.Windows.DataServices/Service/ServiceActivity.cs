using gbc.DAO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SpatialConnect.Windows.DataServices.Service
{
    public class ServiceActivity
    {
        [JsonIgnore]
        public static string PullFileNameFormat = "{0}.pull.json";
        [JsonIgnore]
        public static string PushFileNameFormat = "{0}.push.json";

        public List<GeoRecord> records { get; set; }
        public DateTime created { get; set; }

        public void Write(string path)
        {
            string json = JsonConvert.SerializeObject(this);

            File.WriteAllText(path, json);
        }
    }
}

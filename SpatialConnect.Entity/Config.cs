using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpatialConnect.Entity
{
    public class Config : BaseEntity
    {
        public string arcgis_username { get; set; }
        public string webeoc_position { get; set; }
        public string webeoc_incident { get; set; }
        public string webeoc_password { get; set; }
        public string webeoc_jurisdiction { get; set; }
        public string arcgis_instance { get; set; }
        public string arcgis_database { get; set; }
        public string arcgis_password { get; set; }
        public string arcgis_keyword { get; set; }
        public string arcgis_version { get; set; }
        public string webeoc_username { get; set; }
        public List<string> containers { get; set; }
        public List<string> running_containers { get; set; }
        public string path { get; set; }
        public string type { get; set; }
        public string arcgis_server { get; set; }

        [JsonIgnore]
        public string app_path { get; set; }
        [JsonIgnore]
        public List<string> managed_containers { get; set; }

        public Config()
        {
            managed_containers = new List<string>();
        }

        public void AddManagedContainer(string containerName)
        {
            managed_containers.Add(containerName);
        }

        public void ReleaseManagedContainers()
        {
            foreach (string name in managed_containers)
            {
                if (running_containers.Contains(name))
                {
                    running_containers.Remove(name);
                }
            }
        }
    }
}

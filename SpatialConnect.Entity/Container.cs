using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpatialConnect.Entity
{
    public sealed class Container : BaseEntity
    {
        public int interval { get; set; }
        public string name { get; set; }
        public string format { get; set; }
        public string geometry { get; set; }
        public string push_dir { get; set; }
        public string destination { get; set; }
        public bool transform { get; set; }
        public string source { get; set; }
        public string board { get; set; }
        public string view { get; set; }
        public string source_username { get; set; }
        public string source_password { get; set; }
        public string output_file { get; set; }
        public string path { get; set; }
        public string type { get; set; }
        public string file_in_archive { get; set; }
        public string pull_dir { get; set; }
        public string log_dir { get; set; }
        public string temp_dir { get; set; }
        public string license_type { get; set; }
        public string where_clause { get; set; }
        public bool use_relationships { get; set; }
        public string relationships_dir { get; set; }
        public string key { get; set; }
        public int wkid { get; set; }
        public bool update_only { get; set; }
        public bool has_attachments { get; set; }

        [JsonIgnore]
        public bool working { get; set; }

        [JsonIgnore]
        public Fields Fields { get; set; }

        [JsonIgnore]
        public Config Config { get; set; }

        [JsonIgnore]
        public History PullHistory { get; set; }

        [JsonIgnore]
        public History PushHistory { get; set; }

        [JsonIgnore]
        public Relationships Relationships { get; set; }
    }
}

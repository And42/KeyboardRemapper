using System.Collections.Generic;
using Newtonsoft.Json;

namespace App.Logic.JsonModels
{
    public class ExportedMappings
    {
        [JsonProperty("structure_version")]
        public int StructureVersion { get; set; }

        [JsonProperty("key_mappings")]
        public Dictionary<int, int> KeyMappings { get; set; }
    }
}

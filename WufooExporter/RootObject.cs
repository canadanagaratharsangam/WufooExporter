using System.Collections.Generic;
using Newtonsoft.Json;

namespace WufooExporter
{
    public class RootObject
    {
        [JsonProperty("Entries")]
        public List<Entry> Entries { get; set; }
    }
}
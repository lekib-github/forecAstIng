using Newtonsoft.Json;

namespace forecAstIng.Model
{
    internal class Geocode
    {
        public int place_id { get; set; }
        public string licence { get; set; }
        public string osm_type { get; set; }
        public object osm_id { get; set; }
        public List<string> boundingbox { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
        public string @class { get; set; }
        public string type { get; set; }
        public double importance { get; set; }
    }

    public class Address
    {
        public string house_number { get; set; }
        public string road { get; set; }
        public string quarter { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string state { get; set; }

        [JsonProperty("ISO3166-2-lvl4")]
        public string ISO31662lvl4 { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
    }

    public class Geoloc
    {
        public int place_id { get; set; }
        public string licence { get; set; }
        public string osm_type { get; set; }
        public object osm_id { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
        public Address address { get; set; }
        public List<string> boundingbox { get; set; }
    }
}

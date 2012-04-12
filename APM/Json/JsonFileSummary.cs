
namespace Two10.APM.Json
{
    class JsonFileSummary
    {
        public string type { get; set; }
        public string sha { get; set; }
        public JsonFileLinks _links { get; set; }
        public long size { get; set; }
        public string name { get; set; }
        public string path { get; set; }
    }
}

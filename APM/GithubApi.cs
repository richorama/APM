using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using Two10.APM.Json;

namespace Two10.APM
{
    static class GithubApi
    {

        public static IEnumerable<PluginSummary> GetFileList(string user, string repo, string path)
        {
            var list = Get<JsonFileSummary[]>(string.Format("https://api.github.com/repos/{0}/{1}/contents/{2}", user, repo, path));
            foreach (var file in list)
            {
                yield return new PluginSummary { Name = file.name, Size = file.size };
            }
        }

        public static void GetFile(string user, string repo, string path, string filename, string localfile)
        {
            Download(string.Format(@"https://raw.github.com/{0}/{1}/master/{2}{3}", user, repo, string.IsNullOrEmpty(path) ? "" : path + @"/", filename), localfile);
        }

        private static T Get<T>(string url)
        {
            var request = WebRequest.Create(url);
            request.Method = "GET";
            using (var response = request.GetResponse())
            {
                StreamReader sr = new StreamReader(response.GetResponseStream());
                var jss = new JavaScriptSerializer();
                return jss.Deserialize<T>(sr.ReadToEnd());
            }
        }

        private static void Download(string url, string filename)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, filename);
            }
        }

    }
}

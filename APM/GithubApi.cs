﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using Two10.APM.Json;

namespace Two10.APM
{
    static class GithubApi
    {

        public static IEnumerable<PluginSummary> GetFileList(string user, string repo, string path, string type)
        {
            var list = Get<JsonFileSummary[]>(string.Format("https://api.github.com/repos/{0}/{1}/contents/{2}", user, repo, path));
            foreach (var file in list.Where(x => x.type == type))
            {
                yield return new PluginSummary { Name = file.name, Size = file.size };
            }
        }

        public static void GetFile(string user, string repo, string path, string filename, string localfile)
        {
            string uri = string.Format(@"https://raw.github.com/{0}/{1}/master/{2}{3}", user, repo, string.IsNullOrEmpty(path) ? "" : path + @"/", filename);
            Download(uri, localfile);

        }

        private static T Get<T>(string url)
        {
            var request = WebRequest.Create(url);
            request.Headers.SetUserAgent();
            request.Method = "GET";
            Console.WriteLine(@"GET {0} ", new Uri(url).Segments.Last());
            T output;
            using (var response = request.GetResponse())
            {
                var sr = new StreamReader(response.GetResponseStream());
                var jss = new JavaScriptSerializer();
                output = jss.Deserialize<T>(sr.ReadToEnd());
            }

            return output;
        }

        private static void Download(string url, string filename)
        {
            Console.WriteLine(@"GET {0} ", new Uri(url).Segments.Last());
            using (var client = new WebClient())
            {
                client.Headers.SetUserAgent();
                client.DownloadFile(url, filename);
            }

        }

    }
}

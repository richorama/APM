using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Two10.APM
{
    public static class PluginInfo
    {
        const string schema = @"http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceDefinition";

        private static IEnumerable<string> GetTasks(XDocument xDoc)
        {
            var startup = xDoc.Descendants(XName.Get("Startup", schema)).FirstOrDefault();
            if (null != startup)
            {
                foreach (var item in startup.Descendants(XName.Get("Task", schema)))
                {
                    yield return string.Format(
                        "{0} (executionContext:{1} taskType:{2})",
                        item.ReadAttribute("commandLine"),
                        item.ReadAttribute("executionContext"),
                        item.ReadAttribute("taskType"));
                }
            }
        }

        private static IEnumerable<string> GetCertificates(XDocument xDoc)
        {
            var certificates = xDoc.Descendants(XName.Get("Certificates", schema)).FirstOrDefault();
            if (null != certificates)
            {
                foreach (var item in certificates.Descendants(XName.Get("Certificate", schema)))
                {
                    yield return string.Format(
                        "{0} (storeLocation: {1} storeName:{2} permissionLevel:{3})",
                        item.ReadAttribute("name"),
                        item.ReadAttribute("storeLocation"),
                        item.ReadAttribute("storeName"),
                        item.ReadAttribute("permissionLevel"));
                }
            }
        }

        private static IEnumerable<string> GetEndpoints(XDocument xDoc)
        {
            var endpoints = xDoc.Descendants(XName.Get("Endpoints", schema)).FirstOrDefault();
            if (null != endpoints)
            {
                foreach (var item in endpoints.Descendants(XName.Get("InputEndpoint", schema)))
                {
                    yield return string.Format(
                        "InputEndpoint (name:{0} protocol:{1} port:{2} localPort:{3})",
                        item.ReadAttribute("name"),
                        item.ReadAttribute("protocol"),
                        item.ReadAttribute("port"),
                        item.ReadAttribute("localPort"));
                }
                foreach (var item in endpoints.Descendants(XName.Get("InternalEndpoint", schema)))
                {
                    yield return string.Format(
                        "InternalEndpoint (name:{0} protocol:{1} port:{2} localPort:{3})",
                        item.ReadAttribute("name"),
                        item.ReadAttribute("protocol"),
                        item.ReadAttribute("port"),
                        item.ReadAttribute("localPort"));
                }
            }
        }



        private static IEnumerable<string> GetSettings(XDocument xDoc)
        {
            var @namespace = GetNamespace(xDoc).FirstOrDefault();
            var settings = xDoc.Descendants(XName.Get("ConfigurationSettings", schema)).FirstOrDefault();
            if (null != settings)
            {
                foreach (var item in settings.Descendants(XName.Get("Setting", schema)))
                {
                    yield return string.Format(
                        "{0}.{1}",
                        @namespace,
                        item.ReadAttribute("name"));
                }
            }
        }

        private static IEnumerable<string> GetNamespace(XDocument xDoc)
        {
            var roleModule = xDoc.Descendants(XName.Get("RoleModule", schema)).FirstOrDefault();
            if (null != roleModule)
            {
                yield return roleModule.ReadAttribute("namespace");
            }
        }

        private static IEnumerable<string> GetFiles(string path)
        {
            return Directory.EnumerateFiles(path).Select(x => Path.GetFileName(x));
        }

        private static IEnumerable<string> GetDocumentation(string path)
        {
            if (!File.Exists(path))
            {
                yield break;
            }

            using (var sr = new StreamReader(path))
            {
                yield return "\r\n" + sr.ReadToEnd();
            }
        }



        public static void Info(string name, string csPluginPath, string readmePath)
        {
            Console.WriteLine("Information for {0}", name);

            if (!File.Exists(csPluginPath))
            {
                Console.WriteLine("Cannot find plugin: " + csPluginPath);
                return;
            }

            var xDoc = XDocument.Load(csPluginPath);

            DisplayList("Namespace", GetNamespace(xDoc));
            DisplayList("Tasks", GetTasks(xDoc));
            DisplayList("Settings", GetSettings(xDoc));
            DisplayList("Endpoints", GetEndpoints(xDoc));
            DisplayList("Certificates", GetCertificates(xDoc));
            DisplayList("Files", GetFiles(Path.GetDirectoryName(csPluginPath)));
            DisplayList("Documentation", GetDocumentation(readmePath));
        }



        private static void DisplayList(string name, IEnumerable<string> items)
        {
            Console.WriteLine("{0}:", name);

            if (items.Count() == 0)
            {
                Console.WriteLine("  No {0}", name.ToLower());
                return;
            }
            foreach (var item in items)
            {
                Console.WriteLine("  {0}", item);
            }

        }
    }
}

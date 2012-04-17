using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Two10.APM
{
    public class PluginManager
    {
        private string pluginFolder;
        private string githubUser;
        private string githubRepo;
        private string githubPath;

        public PluginManager(string pluginFolder, string githubUser, string githubRepo, string githubPath)
        {
            this.pluginFolder = pluginFolder;
            this.githubUser = githubUser;
            this.githubRepo = githubRepo;
            this.githubPath = githubPath;
        }

        public IEnumerable<PluginSummary> ListAvailablePlugins()
        {
            return GithubApi.GetFileList(this.githubUser, this.githubRepo, this.githubPath).OrderBy(x => x.Name);
        }

        public IEnumerable<PluginSummary> ListInstalledPlugins()
        {
            var di = new DirectoryInfo(pluginFolder);
            foreach (var directory in di.EnumerateDirectories("*"))
            {
                yield return new PluginSummary { Name = directory.Name, Size = directory.Size() };
            }
        }

        public void InstallPlugin(string name)
        {
            var tempfilename = Path.GetTempFileName();
            var filename = name + ".zip";
            try
            {
                GithubApi.GetFile(githubUser, githubRepo, githubPath, filename, tempfilename);
                UnZip(tempfilename, Path.Combine(this.pluginFolder, name));
                using (new Colour(ConsoleColor.Green))
                {
                    Console.WriteLine("Installed " + name);
                }
            }
            catch (WebException)
            {
                using (new Colour(ConsoleColor.Red))
                {
                    Console.WriteLine("Plugin does not exist in library: " + name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                File.Delete(tempfilename);
            }
        }

        public void RemovePlugin(string name)
        {
            string path = Path.Combine(this.pluginFolder, name);
            if (Directory.Exists(path))
            {
                Extensions.DeleteDirectory(Path.Combine(this.pluginFolder, name));
                using (new Colour(ConsoleColor.Green))
                {
                    Console.WriteLine("Removed " + name);
                }
                return;
            }
            using (new Colour(ConsoleColor.Red))
            {
                Console.WriteLine("Plugin is not installed: " + name);
            }

        }

        public void UpdatePlugin(string name)
        {
            InstallPlugin(name);
        }

        public void UpdateAll()
        {
            foreach (var plugin in ListInstalledPlugins())
            {
                Console.WriteLine("Updating " + plugin.DisplayName);
                UpdatePlugin(plugin.DisplayName);
            }
        }

        private void UnZip(string zipFile, string destinationFolder)
        {
            var info = new ProcessStartInfo
            {
                //WorkingDirectory = @"D:\git\APM\APM\",
                Arguments = string.Format("x -y -o\"{0}\" \"{1}\"", destinationFolder, zipFile),
                FileName = "7za.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var process = Process.Start(info);
            process.WaitForExit();
            if (0 != process.ExitCode)
            {
                Console.WriteLine(process.StandardOutput.ReadToEnd());
                throw new ApplicationException("7zip exited with error code " + process.ExitCode.ToString());
            }
        }

        const string schema = @"http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceDefinition";

        private IEnumerable<string> GetTasks(XDocument xDoc)
        {
            var startup = xDoc.Descendants(XName.Get("Startup", schema)).FirstOrDefault();
            if (null != startup)
            {
                foreach (var item in startup.Descendants(XName.Get("Task", schema)))
                {
                    yield return string.Format(
                        "  {0} executionContext:{1} taskType:{2}",
                        item.Attribute("commandLine").Value,
                        item.Attribute("executionContext").Value,
                        item.Attribute("taskType").Value);
                }
            }
        }

        private IEnumerable<string> GetCertificates(XDocument xDoc)
        {
            var certificates = xDoc.Descendants(XName.Get("Certificates", schema)).FirstOrDefault();
            if (null != certificates)
            {
                foreach (var item in certificates.Descendants(XName.Get("Certificate", schema)))
                {
                    yield return string.Format(
                        "  {0} storeLocation: {1} storeName:{2} permissionLevel:{3}",
                        item.Attribute("name").Value,
                        item.Attribute("storeLocation").Value,
                        item.Attribute("storeName").Value,
                        item.Attribute("permissionLevel").Value);
                }
            }
        }

        private IEnumerable<string> GetEndpoints(XDocument xDoc)
        {
            var endpoints = xDoc.Descendants(XName.Get("Endpoints", schema)).FirstOrDefault();
            if (null != endpoints)
            {
                foreach (var item in endpoints.Descendants(XName.Get("InputEndpoint", schema)))
                {
                    yield return string.Format(
                        "  InputEndpoint {0} protocol:{1} port:{2} localPort:{3}",
                        item.Attribute("name").Value,
                        item.Attribute("port").Value,
                        item.Attribute("localPort").Value);
                }
                foreach (var item in endpoints.Descendants(XName.Get("InternalEndpoint", schema)))
                {
                    yield return string.Format(
                        "  InternalEndpoint {0} executionContext:{1} taskType:{2}",
                        item.Attribute("commandLine").Value,
                        item.Attribute("executionContext").Value,
                        item.Attribute("taskType").Value);
                }
            }
        }

        private IEnumerable<string> GetSettings(XDocument xDoc)
        {
            var @namespace = GetNamespace(xDoc);
            var settings = xDoc.Descendants(XName.Get("ConfigurationSettings", schema)).FirstOrDefault();
            if (null != settings)
            {
                foreach (var item in settings.Descendants(XName.Get("Setting", schema)))
                {
                    yield return string.Format(
                        "  {0}.{1}",
                        @namespace,
                        item.Attribute("name").Value);
                }
            }
        }

        private string GetNamespace(XDocument xDoc)
        {
            var roleModule = xDoc.Descendants(XName.Get("RoleModule", schema)).FirstOrDefault();
            if (null != roleModule)
            {
                return roleModule.Attribute("namespace").Value;
            }
            return null;
        }

        public void Info(string name)
        {
            Console.WriteLine("Information for {0}", name);
            string csplugin = Path.Combine(this.pluginFolder, name, name + ".csplugin");

            if (!File.Exists(csplugin))
            {
                Console.WriteLine("Cannot find module");
                Console.WriteLine("Cannot find " + csplugin);
                return;
            }

            var xDoc = XDocument.Load(csplugin);

            Console.WriteLine("Namespace:");
            Console.WriteLine("  {0}", GetNamespace(xDoc));
            Console.WriteLine("Tasks:");
            foreach (var task in GetTasks(xDoc))
            {
                Console.WriteLine(task);
            }

            Console.WriteLine("Settings:");
            foreach (var setting in GetSettings(xDoc))
            {
                Console.WriteLine(setting);
            }

            Console.WriteLine("Endpoints:");
            foreach (var endpoint in GetEndpoints(xDoc))
            {
                Console.WriteLine(endpoint);
            }

            Console.WriteLine("Certificates:");
            foreach (var certificate in GetCertificates(xDoc))
            {
                Console.WriteLine(certificate);
            }

            Console.WriteLine("Documentation:");
            string readme = Path.Combine(this.pluginFolder, name, "readme.txt");
            if (!File.Exists(readme))
            {
                Console.WriteLine("  No documentation exists for this module");
                Console.WriteLine("  Cannot find " + readme);
                return;
            }

            using (StreamReader sr = new StreamReader(readme))
            {
                Console.WriteLine(sr.ReadToEnd());
            }
        }
    }
}

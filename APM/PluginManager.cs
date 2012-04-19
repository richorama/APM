using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

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
            bool dirExists = Directory.Exists(Path.Combine(this.pluginFolder, name));
            bool error = false;
            try
            {
                Directory.CreateDirectory(Path.Combine(this.pluginFolder, name));
                foreach (var file in GithubApi.GetFileList(githubUser, githubRepo, string.Format("{0}/{1}", githubPath, name)))
                {

                    Console.Write(@"GET {0}/{1}", name, file.Name);
                    GithubApi.GetFile(githubUser, githubRepo, string.Format("{0}/{1}", githubPath, name), file.Name, Path.Combine(this.pluginFolder, name, file.Name));
                    using (new Colour(ConsoleColor.Green))
                    {
                        Console.WriteLine(" OK");
                    }

                }
                using (new Colour(ConsoleColor.Green))
                {
                    Console.WriteLine("Installed {0}", name);
                }
            }
            catch (WebException)
            {
                error = true;
                using (new Colour(ConsoleColor.Red))
                {
                    Console.WriteLine("Plugin does not exist in library: " + name);
                }
            }
            catch (Exception ex)
            {
                error = true;
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (error && !dirExists && Directory.Exists(Path.Combine(this.pluginFolder, name)))
                {
                    // roll back 
                    Extensions.DeleteDirectory(Path.Combine(this.pluginFolder, name));
                }
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

        public void Info(string name)
        {
            PluginInfo.Info(
                name,
                Path.Combine(this.pluginFolder, name, name + ".csplugin"),
                Path.Combine(this.pluginFolder, name, "readme.txt"));
        }



    }
}

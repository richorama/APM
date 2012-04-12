using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
                UpdatePlugin(plugin.DisplayName);
            }
        }

        private void UnZip(string zipFile, string destinationFolder)
        {
            var info = new ProcessStartInfo
            {
                WorkingDirectory = @"D:\git\APM\APM\",
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


    }
}

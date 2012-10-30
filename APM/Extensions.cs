using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Two10.APM
{
    public static class Extensions
    {
        public static long Size(this DirectoryInfo d)
        {
            long total = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                total += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                total += Size(di);
            }
            return (total);
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        public static string GetSDKPath(string sdkVersion = null)
        {
            // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\ServiceHosting\
            try
            {
                var key = Registry.LocalMachine
                    .OpenSubKey("SOFTWARE")
                    .OpenSubKey("Microsoft")
                    .OpenSubKey("Microsoft SDKs")
                    .OpenSubKey("ServiceHosting");



                if (string.IsNullOrWhiteSpace(sdkVersion))
                {
                    sdkVersion = key.GetSubKeyNames().OrderByDescending(x => x).FirstOrDefault(); // i.e. v1.6
                }
                else
                {
                    if (!sdkVersion.ToLower().StartsWith("v"))
                    {
                        sdkVersion = "v" + sdkVersion;
                    }
                }

                if (null == sdkVersion)
                {
                    throw new NullReferenceException("Cannot find an SDK in the registry");
                }

                var path = (string)key.OpenSubKey(sdkVersion).GetValue("InstallPath");
                path = Path.Combine(path, "bin", "plugins");
                if (!Directory.Exists(path))
                {
                    throw new ApplicationException("Cannot find plugins folder: " + path);
                }
                return path;
            }
            catch
            {
                throw new ApplicationException("SDK cannot be located");
            }
        }

        public static string ReadAttribute(this XElement value, string name)
        {
            var attribute = value.Attribute(XName.Get(name));
            if (null == attribute)
            {
                return "";
            }
            return attribute.Value;
        }

    }
}

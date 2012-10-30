using System;
using System.Collections.Generic;
using System.Linq;

namespace Two10.APM
{
    public class Program
    {

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"
AZURE PLUGIN MANAGER (c) 2012
Richard Astbury Two10Degrees.com

Usage:

apm list                  Displays a list of plugins available in the library
apm installed             Displays a list of installed plugins
apm install [PluginName]  Installs the specified plugin (case sensitive)
apm remove [PluginName]   Removes the specified plugin
apm update [PluginName]   Updated the specified plugin
apm update                Updates all plugins
apm info [PluginName]     Displays information about a plugin

For example:

> apm install ClassicASP

Additional options:

-u [username]             GitHub username containing plugins (i.e. richorama)
-r [repository]           GitHub respository (i.e. AzurePluginLibrary)
-p [path]                 Repository path containing plugins (i.e. plugins)
-v [version]              The SDK version you wish to work with (i.e. 1.7)

For example:

> apm install ClassicASP -u richorama -r AzurePluginLibrary -p plugins

Installed plugins can be included in your ServiceDefinition.csdef file:

<ServiceDefinition>
  <WorkerRole>
    <Imports>
      <Import moduleName=""[PluginName]"" />
    </Imports>
  </WorkerRole>
</ServiceDefinition>

APM must be ran elevated to modify the local plugins (as administrator).
The Windows Azure SDK must be installed on the local machine.");
                return;
            }

            try
            {
                var user = GetSwitch(args, "-u") ?? "richorama";
                var respository = GetSwitch(args, "-r") ?? "AzurePluginLibrary";
                var path = GetSwitch(args, "-p") ?? "plugins";
                var sdkVersion = GetSwitch(args, "-v") ?? "";

                var sdkPath = Extensions.GetSDKPath(sdkVersion);
                var manager = new PluginManager(sdkPath, user, respository, path);

                switch (args[0].ToLower())
                {
                    case "ls":
                    case "list":
                        var plugins = manager.ListAvailablePlugins().ToArray();
                        Console.WriteLine();
                        Console.WriteLine("Available plugins:");
                        Console.WriteLine();
                        foreach (var plugin in plugins)
                        {
                            Console.WriteLine(plugin);
                        }
                        break;
                    case "installed":
                        Console.WriteLine("Installed plugins:");
                        Console.WriteLine();
                        foreach (var plugin in manager.ListInstalledPlugins())
                        {
                            Console.WriteLine(plugin);
                        }
                        break;
                    case "get":
                    case "install":
                        if (args.Length < 2)
                        {
                            using (new Colour(ConsoleColor.Red))
                            {
                                Console.WriteLine("You must supply the name of the plugin to install");
                                break;
                            }
                        }
                        PermissionsWarning();
                        Console.WriteLine("Installing " + args[1]);
                        manager.InstallPlugin(args[1]);
                        break;
                    case "update":
                        PermissionsWarning();
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Updating all plugins");
                            manager.UpdateAll();
                            break;
                        }
                        Console.WriteLine("Updating " + args[1]);
                        manager.UpdatePlugin(args[1]);
                        break;
                    case "?":
                    case "help":
                    case "info":
                        if (args.Length < 2)
                        {
                            using (new Colour(ConsoleColor.Red))
                            {
                                Console.WriteLine("You must supply the name of the plugin");
                                break;
                            }
                        }
                        manager.Info(args[1]);
                        break;
                    case "delete":
                    case "del":
                    case "remove":
                        if (args.Length < 2)
                        {
                            using (new Colour(ConsoleColor.Red))
                            {
                                Console.WriteLine("You must supply the name of the plugin to remove");
                                break;
                            }
                        }
                        PermissionsWarning();
                        Console.WriteLine("Removing " + args[1]);
                        manager.RemovePlugin(args[1]);
                        break;
                    default:
                        using (new Colour(ConsoleColor.Red))
                        {
                            Console.WriteLine("Command not found: {0}", args[0]);
                        }
                        Console.WriteLine("Type APM for help");
                        break;
                }
            }
            catch (Exception ex)
            {
                using (new Colour(ConsoleColor.Red))
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static void PermissionsWarning()
        {
            if (UacHelper.IsUacEnabled && !UacHelper.IsProcessElevated)
            {
                using (new Colour(ConsoleColor.Red))
                {
                    Console.WriteLine("APM is not running elevated. This action is likely to fail.");
                }
            }
        }

        public static string GetSwitch(string[] args, string name)
        {
            if (null == args) throw new ArgumentNullException("args");
            if (null == name) throw new ArgumentNullException("name");

            var argsList = new List<string>(args.Select(x => x.ToLower()));
            var index = argsList.IndexOf(name.ToLower());
            if (index == -1)
            {
                return null;
            }
            if (args.Length < index + 2)
            {
                return null;
            }
            return args[index + 1];
        }



    }
}

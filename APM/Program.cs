using System;

namespace Two10.APM
{
    class Program
    {

        static void Main(string[] args)
        {


            if (args.Length == 0)
            {
                Console.WriteLine(@"
AZURE PLUGIN MANAGER (c) 2012
Richard Astbury Two10Degrees.com

Usage:

apm list                  displays a list of plugins available in the library
apm installed             displays a list of installed plugins
apm install [PluginName]  installs the specified plugin
apm remove [PluginName]   removes the specified plugin
apm update [PluginName]   updated the specified plugin
apm update                updates all plugins

For example:

apm install ClassicASP

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
                var sdkPath = Extensions.GetSDKPath();
                var manager = new PluginManager(sdkPath, "richorama", "AzurePluginLibrary", "plugins");

                switch (args[0].ToLower())
                {
                    case "list":
                        Console.WriteLine("Available plugins:");
                        foreach (var plugin in manager.ListAvailablePlugins())
                        {
                            Console.WriteLine(plugin);
                        }
                        break;
                    case "installed":
                        Console.WriteLine("Installed plugins:");
                        foreach (var plugin in manager.ListInstalledPlugins())
                        {
                            Console.WriteLine(plugin);
                        }
                        break;
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



    }
}

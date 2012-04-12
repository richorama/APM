using System;

namespace Two10.APM
{
    class Program
    {
        const string SDK_PATH = @"C:\Program Files\Windows Azure SDK\v1.6\bin\plugins";

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

The application must be run under elevation to modify the local plugins
");
                return;
            }

            try
            {

                var manager = new PluginManager(SDK_PATH, "richorama", "AzurePluginLibrary", "plugins");

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
                            }
                        }
                        Console.WriteLine("Installing " + args[1]);
                        manager.InstallPlugin(args[1]);
                        break;
                    case "update":
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
                            }
                        }
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





    }
}

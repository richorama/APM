﻿using System.Linq;
using NUnit.Framework;
using Two10.APM;

namespace APM.Tests
{
    [TestFixture]
    public class PluginManagerTests
    {
        [Test]
        public void TestRepoQuery()
        {
            var manager = new PluginManager("", "richorama", "AzurePluginLibrary", "plugins");
            var plugins = manager.ListAvailablePlugins();
            var classicASP = plugins.Where(x => x.DisplayName == "ClassicASP").FirstOrDefault();
            Assert.IsNotNull(classicASP);
        }

        [Test]
        public void TestPluginInstall()
        {
            var manager = new PluginManager(@".\", "richorama", "AzurePluginLibrary", "plugins");
            manager.InstallPlugin("ClassicASP");
        }

        [Test]
        public void TestListInstalled()
        {
            var manager = new PluginManager(@"D:\git\APM\APM\", "richorama", "AzurePluginLibrary", "plugins");
            var plugins = manager.ListInstalledPlugins().ToArray();
        }

        [Test]
        public void TestPluginSummary()
        {
            var summary = new PluginSummary { Name = "Foo.bar" };
            Assert.AreEqual("Foo", summary.DisplayName);
        }

    }
}
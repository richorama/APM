using System.Linq;
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
            Assert.IsNotNullOrEmpty(summary.ToString());
        }

        [Test]
        public void UACHelper()
        {
            Assert.IsTrue(UacHelper.IsUacEnabled);
            Assert.IsFalse(UacHelper.IsProcessElevated);
        }


        [Test]
        public void PluginInfo()
        {
            var manager = new PluginManager(Extensions.GetSDKPath(), "richorama", "AzurePluginLibrary", "plugins");
            manager.Info("Connect");
        }

        [Test]
        public void GetSetting()
        {
            var value = Program.GetSwitch(new string[] { "-r", "repositoryName" }, "-r");
            Assert.AreEqual("repositoryName", value);

            value = Program.GetSwitch(new string[] { "-u", "bar", "-r" }, "-r");
            Assert.IsNull(value);

            value = Program.GetSwitch(new string[] { "-R", "repositoryName" }, "-r");
            Assert.AreEqual("repositoryName", value);

            value = Program.GetSwitch(new string[] { "-R", "repositoryName" }, "-p");
            Assert.IsNull(value);

        }

    }
}

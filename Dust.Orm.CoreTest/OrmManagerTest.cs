using Dust.ORM.Core;
using Dust.ORM.Core.Models;
using Dust.ORM.Core.Repositories;
using Dust.ORM.CoreTest.Models;
using Dust.ORM.UnitTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest
{
    public class OrmManagerTest
    {

        private readonly TestLogger Log;
        public OrmManagerTest(ITestOutputHelper output)
        {
            Log = new TestLogger(output);
        }

        [Fact]
        public void OrmManagerConfigurationTest()
        {
            File.WriteAllText("CONFIG_UNREADABLE.xml", "TEST OF CONFIGURATION");

            ORMManager manager = null;
            Assert.Throws<ConfigurationException>(() => manager = new ORMManager(Log, "FileName&#.:\t\n/\\"));

            manager = new ORMManager(Log, "CONFIG_OK.xml");
            Assert.True(File.Exists("CONFIG_OK.xml"));

            manager.Config.SelectedDatabase = "Not a database name";
            DataRepository<TestClass<int>> repo = null;
            Assert.Throws<ConfigurationException>(() => repo = manager.Get<TestClass<int>>());
            manager.Config.Configs.Clear();
            manager.Config.SelectedDatabase = "test"; // Correct database name
            Assert.Throws<ConfigurationException>(() => manager.Get<TestClass<int>>());

            manager = new ORMManager(Log, "CONFIG_UNREADABLE.xml");
            Assert.True(File.Exists("CONFIG_UNREADABLE.xml"));
            Assert.True(File.Exists("old_CONFIG_UNREADABLE.xml"));

            File.Delete("CONFIG_OK.xml");
            File.Delete("CONFIG_UNREADABLE.xml");
            File.Delete("old_CONFIG_UNREADABLE.xml");
        }

    }
}

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

        [Fact]
        public void OrmManagerBasicTest()
        {
            ORMManager manager = new ORMManager(Log, "TestConfiguration.xml");
            
            var repo = manager.Get<TestClass<int>>();
            Assert.NotNull(repo);


            int id=5, testValue1 = 42, testValue2 = 100;
            Assert.True(repo.Insert(new TestClass<int>(id, testValue1, testValue2)));
            Assert.False(repo.Insert(new TestClass<int>(id, testValue1, testValue2)));

            TestClass<int> value = repo.Get(id);
            manager.ResolveReference(ref value);

            Assert.Equal(testValue1, value.TestValue1);
            Assert.Equal(testValue2, value.TestValue2);

            Assert.False(repo.Edit(new TestClass<int>(0, 0, 0)));
            Assert.True(repo.Edit(new TestClass<int>(id, testValue1 + 1, 100)));
            Assert.Equal(testValue1 +1, repo.Get(id).TestValue1);
            
            Assert.True(repo.Delete(id));
            Assert.False(repo.Delete(id));
            Assert.False(repo.Edit(new TestClass<int>(id, testValue1 + 1, 100)));

            Assert.Null(repo.Get(id));

        }

        [Fact]
        public void OrmManagerReferenceTest()
        {
            ORMManager manager = new ORMManager(Log, "TestConfiguration.xml");

            var repo1 = manager.Get<ReferenceModel>();
            var repo2 = manager.Get<SubReferenceModel>();

            int subReferenceID = 10;

            repo2.Insert(new SubReferenceModel(subReferenceID, 42));
            repo1.Insert(new ReferenceModel(0, 4200, subReferenceID, null)); // null is type of SubReferenceModel

            var value1 = repo1.Get(0);
            manager.ResolveReference(ref value1);

            Assert.NotNull(value1.LinkValue_ref);

            repo1.Insert(new ReferenceModel(1, 4200, 100, null)); // 100 is not present in database
            var value2 = repo1.Get(1);
            manager.ResolveReference(ref value2);
            Assert.Null(value2.LinkValue_ref);
        }
    }
}

using Dust.ORM.Core;
using Dust.ORM.Core.Models;
using Dust.ORM.CoreTest.Models;
using Dust.ORM.UnitTest;
using System;
using System.Collections.Generic;
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
        public void OrmManagerBasicTest()
        {
            ORMManager manager = new ORMManager(Log, "TestConfiguration.xml");
            
            var repo = manager.Get<TestClass<int>>();
            Assert.NotNull(repo);


            int id=5, testValue1 = 42, testValue2 = 100;
            Assert.True(repo.Insert(new TestClass<int>(id, testValue1, testValue2)));

            Assert.Equal(testValue1, repo.Get(id).TestValue1);
            Assert.Equal(testValue2, repo.Get(id).TestValue2);

            Assert.True(repo.Delete(id));
            Assert.False(repo.Delete(id));

            Assert.Null(repo.Get(id));
        }
    }
}

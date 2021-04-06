using Dust.ORM.Core;
using Dust.ORM.Core.Repositories;
using Dust.ORM.CoreTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest.Core
{
    [Collection("TestDatabase")]
    public abstract class OrmCoreTestBasic : OrmCoreTest
    {

        protected OrmCoreTestBasic(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void OrmManagerBasicTest()
        {
            SetupOrm();
            Assert.NotNull(Manager);

            try
            {
                var repo = Manager.Get<TestClass<int>>();
                Assert.NotNull(repo);


                int id = 5, testValue1 = 42, testValue2 = 100;
                Assert.True(repo.Insert(new TestClass<int>(id, testValue1, testValue2)));
                Assert.False(repo.Insert(new TestClass<int>(id, testValue1, testValue2)));

                TestClass<int> value = repo.Get(id);

                Assert.Equal(testValue1, value.TestValue1);
                Assert.Equal(testValue2, value.TestValue2);

                Assert.False(repo.Edit(new TestClass<int>(0, 0, 0)));
                Assert.True(repo.Edit(new TestClass<int>(id, testValue1 + 1, 100)));
                Assert.Equal(testValue1 + 1, repo.Get(id).TestValue1);

                Assert.True(repo.Delete(id));
                Assert.False(repo.Delete(id));
                Assert.False(repo.Edit(new TestClass<int>(id, testValue1 + 1, 100)));

                Assert.Null(repo.Get(id));

            }
            catch(ORMException e)
            {
                Log.Info(e.ToString());
                Assert.True(false);
            }

        }

        [Fact]
        public void InsertListTest()
        {
            SetupOrm();
            DataRepository<TestClass<int>> repo = Manager.Get<TestClass<int>>();

            List<TestClass<int>> list = new List<TestClass<int>>();
            for (int i = 1; i < 11; ++i)
            {
                list.Add(new TestClass<int>());
            }

            Assert.True(repo.Clear());
            Assert.True(repo.InsertAll(list));
            List<TestClass<int>> vars = repo.GetAll(-1);
            Assert.Equal(list.Count, vars.Count);
            Assert.True(repo.Clear());
        }

    }
}

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

                Assert.True(repo.InsertAll(new List<TestClass<int>>() {
                    new TestClass<int>(1, testValue1, testValue2),
                    new TestClass<int>(2, testValue1, testValue2),
                    new TestClass<int>(3, testValue1, testValue2),
                    new TestClass<int>(4, testValue1, testValue2),
                    new TestClass<int>(5, testValue1, testValue2),
                    new TestClass<int>(6, testValue1, testValue2),
                    new TestClass<int>(7, testValue1, testValue2),
                    new TestClass<int>(8, testValue1, testValue2),
                    new TestClass<int>(9, testValue1, testValue2),
                    new TestClass<int>(10, testValue1, testValue2),
                    new TestClass<int>(11, testValue1, testValue2),
                    new TestClass<int>(12, testValue1, testValue2),
                }));
                
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
            for (int i = 0; i < 10; ++i)
            {
                list.Add(new TestClass<int>());
            }
            for (int repeat = 0; repeat < 10; ++repeat)
            {
                Assert.True(repo.Clear());
                Assert.True(repo.InsertAll(list));
                List<TestClass<int>> vars = repo.GetAll(-1);
                Assert.Equal(list.Count, vars.Count);
                repo.Clear();
            }
        }

    }
}

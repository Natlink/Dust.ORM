using Dust.ORM.Core;
using Dust.ORM.CoreTest.Models;
using Dust.ORM.UnitTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest.Core
{
    [Collection("Basic")]
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
                Manager.ResolveReference(ref value);

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
                Log.Log(e.ToString());
                Assert.True(false);
            }

        }

        [Fact]
        public void TestBenchmark()
        {
            try
            {
                var res = Benchmark<TestClass<int>>(1, 10, 100, 1000, 10000);//, 10000);

                Log.Log(BenchmarkToString<TestClass<int>>(res));
            }
            catch (Exception e)
            {
                Log.Log(e.ToString());
            }
        }

    }
}

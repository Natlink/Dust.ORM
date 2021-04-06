using Dust.ORM.Core;
using Dust.ORM.Core.Repositories;
using Dust.ORM.CoreTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest.Core
{

    [Collection("TestDatabase")]
    public abstract class OrmCoreTestEnumerable : OrmCoreTest
    {
        protected OrmCoreTestEnumerable(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void OrmManagerEnumerableTest()
        {
            SetupOrm();
            Assert.NotNull(Manager);

            try
            {
                var repo1 = Manager.Get<EnumerableModel>();

                repo1.Insert(new EnumerableModel(1, new List<int>(new int[] { 0, 1, 2, 3, 42 }), 50));

                var value1 = repo1.Get(1);

                Assert.NotNull(value1);
                Assert.NotNull(value1.Datas);
                Assert.Equal(5, value1.Datas.Count);

                Assert.Equal(0, value1.Datas[0]);
                Assert.Equal(1, value1.Datas[1]);
                Assert.Equal(2, value1.Datas[2]);
                Assert.Equal(3, value1.Datas[3]);
                Assert.Equal(42, value1.Datas[4]);
            }
            catch (ORMException e)
            {
                Log.Info(e.ToString());
                Assert.True(false);
            }
        }


        [Fact]
        public void InsertListTest()
        {
            SetupOrm();
            DataRepository<EnumerableModel> repo = Manager.Get<EnumerableModel>();

            List<EnumerableModel> list = new List<EnumerableModel>();
            for (int i = 0; i < 10; ++i)
            {
                list.Add(new EnumerableModel(0, new List<int>(new int[] { 0, 1, 2, 3, 42 }), 50));
            }
            for (int repeat = 0; repeat < 10; ++repeat)
            {
                Assert.True(repo.Clear());
                Assert.True(repo.InsertAll(list));
                List<EnumerableModel> vars = repo.GetAll(0);
                Assert.Equal(list.Count, vars.Count);
                Assert.True(repo.Clear());
            }
        }
    }
}

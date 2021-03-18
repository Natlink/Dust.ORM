using Dust.ORM.Core;
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

    [Collection("Enumerable")]
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
                Log.Log(e.ToString());
                Assert.True(false);
            }
        }
    }
}

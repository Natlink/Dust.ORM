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
    public abstract class OrmCoreTestParsable : OrmCoreTest
    {
        protected OrmCoreTestParsable(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SimpleParsableTest()
        {
            SetupOrm();
            Assert.NotNull(Manager);

            try
            {
                var repo1 = Manager.Get<ParsableModel>();

                Assert.True(repo1.Insert(new ParsableModel(1, 50, new ParsableReference(100, 200, 300))));

                var value1 = repo1.Get(1);
                Assert.NotNull(value1);
                Assert.NotNull(value1.ParsableObject);

                Assert.Equal(100, value1.ParsableObject.Value1);
                Assert.Equal(200, value1.ParsableObject.Value2);
                Assert.Equal(300, value1.ParsableObject.Value3);
            }
            catch (ORMException e)
            {
                Log.Info(e.ToString());
                Assert.True(false);
            }
        }

        [Fact]
        public void NullParsableTest()
        {
            SetupOrm();
            Assert.NotNull(Manager);

            try
            {
                var repo1 = Manager.Get<ParsableModel>();

                Assert.False(repo1.Insert(null));

                Assert.True(repo1.Insert(new ParsableModel(1, 50, null)));

                var value1 = repo1.Get(1);
                Assert.NotNull(value1);
                Assert.Null(value1.ParsableObject);
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
            DataRepository<ParsableModel> repo = Manager.Get<ParsableModel>();

            List<ParsableModel> list = new List<ParsableModel>();
            for (int i = 1; i < 11; ++i)
            {
                list.Add(new ParsableModel(0, 50, new ParsableReference(100, 200, 300)));
            }
            Assert.True(repo.Clear());
            Assert.True(repo.InsertAll(list));
            List<ParsableModel> vars = repo.GetAll(0);
            Assert.Equal(list.Count, vars.Count);
            Assert.True(repo.Clear());
        }
    }
}

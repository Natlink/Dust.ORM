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
    [Collection("Parsable")]
    public abstract class OrmCoreTestParsable : OrmCoreTest
    {
        protected OrmCoreTestParsable(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void OrmManagerParsableTest()
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
                Log.Log(e.ToString());
                Assert.True(false);
            }
        }
    }
}

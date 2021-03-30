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
    [Collection("Reference")]
    public abstract class OrmCoreTestReference : OrmCoreTest
    {
        protected OrmCoreTestReference(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void OrmManagerReferenceTest()
        {
            SetupOrm();
            Assert.NotNull(Manager);

            try
            {
                var repo1 = Manager.Get<ReferenceModel>();
                var repo2 = Manager.Get<SubReferenceModel>();

                int subReferenceID = 10;

                Assert.True(repo2.Insert(new SubReferenceModel(subReferenceID, 42)));
                Assert.True(repo1.Insert(new ReferenceModel(1, 4200, subReferenceID, null))); // null is type of SubReferenceModel

                var value1 = repo1.Get(1);

                Assert.NotNull(value1.LinkValue_ref);

                repo1.Insert(new ReferenceModel(2, 4200, 100, null)); // 100 is not present in database
                var value2 = repo1.Get(2);
                Assert.Null(value2.LinkValue_ref);
            }
            catch (ORMException e)
            {
                Log.Info(e.ToString());
                Assert.True(false);
            }
        }
    }
}

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


        [Fact]
        public void InsertListTest()
        {
            SetupOrm();
            DataRepository<ReferenceModel> repo1 = Manager.Get<ReferenceModel>();
            DataRepository<SubReferenceModel> repo2 = Manager.Get<SubReferenceModel>();

            List<ReferenceModel> list1 = new List<ReferenceModel>();
            List<SubReferenceModel> list2 = new List<SubReferenceModel>();
            for (int i = 1; i < 11; ++i)
            {
                list2.Add(new SubReferenceModel(i, 42));
                list1.Add(new ReferenceModel(i, 4200, i, null));
            }
            repo1.Clear();
            repo2.Clear();
            repo2.InsertAll(list2);
            repo1.InsertAll(list1);
            List<ReferenceModel> vars1 = repo1.GetAll(0);
            Assert.Equal(list1.Count, vars1.Count);
            repo1.Clear();
            repo2.Clear();
        }

    }
}

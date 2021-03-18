using Dust.ORM.Core;
using Dust.ORM.UnitTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest.Core
{
    public abstract class OrmCoreTest
    {
        protected ORMManager Manager;
        protected readonly TestLogger Log;

        public OrmCoreTest(ITestOutputHelper output)
        {
            Log = new TestLogger(output);
        }

        public abstract void SetupOrm();


    }
}

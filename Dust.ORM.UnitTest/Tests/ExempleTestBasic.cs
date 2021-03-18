using Dust.ORM.Core;
using Dust.ORM.CoreTest.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dust.ORM.CoreTest.Tests
{
    public class ExempleTestBasic : OrmCoreTestBasic
    {

        public ExempleTestBasic(ITestOutputHelper output) : base(output)
        {
        }

        public override void SetupOrm()
        {
            Manager = new ORMManager(Log, "TestConfigurationBasic.xml");
        }

    }
}

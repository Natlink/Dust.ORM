using Dust.ORM.Core;
using Dust.ORM.CoreTest.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;

namespace Dust.ORM.CoreTest.Tests
{
    public class ExempleTestReference : OrmCoreTestReference
    {
        public ExempleTestReference(ITestOutputHelper output) : base(output)
        {
        }

        public override void SetupOrm()
        {
            Manager = new ORMManager(Log, "TestConfigurationReference.xml");
        }
    }
}
